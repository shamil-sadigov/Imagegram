using Imagegram.Database;
using Imagegram.Database.Entities;
using Imagegram.Features.Posts.CreatePost.Services;
using MediatR;

namespace Imagegram.Features.Posts.CreatePost;

// TODO: Add logging everywhere

public class CreatePostRequestHandler : IRequestHandler<CreatePostCommand, PostDto>
{
    private readonly ImageProcessor _imageProcessor;
    private readonly IImageStorage _imageStorage;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<CreatePostRequestHandler> _logger;
    private readonly ISystemTime _systemTime;
    
    public CreatePostRequestHandler(
        ILogger<CreatePostRequestHandler> logger, 
        ImageProcessor imageProcessor,
        IImageStorage imageStorage,
        ApplicationDbContext dbContext, 
        ISystemTime systemTime)
    {
        _logger = logger;
        _imageProcessor = imageProcessor;
        _imageStorage = imageStorage;
        _dbContext = dbContext;
        _systemTime = systemTime;

        // TODO: We need to ensure that before deployment blob containers exists
    }
    
    public async Task<PostDto> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        Stream originalImageStream = request.Image.OpenReadStream();
        await using Stream processedImageStream = _imageProcessor.ProcessImage(originalImageStream);
        
        originalImageStream.Position = 0;
        processedImageStream.Position = 0;

        await EnsureDatabaseIsHealthyAsync(cancellationToken);
        
        (SavedImage originalImage, SavedImage processedImage) 
            = await SaveImagesAsync(request.UserId, originalImageStream, processedImageStream, cancellationToken);
        
        var post = await CreatePostAsync(request, originalImage, processedImage, cancellationToken);

        return MapToDto(post);
    }
    
    /// <summary>
    /// Lightweight protection to ensure that DB is available, so that when we save image in BlobStorage
    /// we are able to save uri of that image in DB.
    /// 
    /// Because otherwise if we don't check DB availability we  could store image in BlobStorage and afterward find out
    /// that DB is not available, and unable to save image url.
    /// </summary>
    private async Task EnsureDatabaseIsHealthyAsync(CancellationToken cancellationToken)
    {
        if (!await _dbContext.Database.CanConnectAsync(cancellationToken))
        {
            throw new InvalidOperationException("Database is not healthy");
        }
    }

    private async Task<Post> CreatePostAsync(
        CreatePostCommand command,
        SavedImage originalImage, 
        SavedImage processedImage,
        CancellationToken cancellationToken)
    {
        var post = new Post(
            command.UserId, 
            command.Description, 
            new ImageInfo(originalImage.Name, originalImage.Uri),
            new ImageInfo(processedImage.Name, processedImage.Uri), 
            _systemTime.CurrentUtc);
        
        await _dbContext.Posts.AddAsync(post, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return post;
    }

    private async Task<(SavedImage, SavedImage)> SaveImagesAsync(
        int userId,
        Stream originalImageStream,
        Stream processedImageStream,
        CancellationToken token)
    {
        var imageName = $"{userId}-{Guid.NewGuid()}.jpg";
        
        var savingOriginalImage = _imageStorage.SaveOriginalImageAsync(imageName, originalImageStream, token);
        var savingProcessedImage = _imageStorage.SaveProcessedImageAsync(imageName, processedImageStream, token);

        await Task.WhenAll
        (
            savingOriginalImage,
            savingProcessedImage
        );
        
        return (savingOriginalImage.Result, savingProcessedImage.Result);
    }
    
    private static PostDto MapToDto(Post post)
    {
        return new PostDto(
            post.Id,
            post.CreatedBy,
            post.CommentCount,
            post.LastTimeUpdatedAt,
            post.CreatedAt,
            post.Description,
            post.Image.ProcessedImage.Uri,
            null);
    }
}