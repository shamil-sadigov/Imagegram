using Imagegram.Database;
using Imagegram.Database.Entities;
using Imagegram.Features.Posts.CreatePost.Services;
using MediatR;

namespace Imagegram.Features.Posts.CreatePost;

public class CreatePostRequestHandler : IRequestHandler<CreatePostCommand, CreatedPost>
{
    private readonly ImageProcessor _imageProcessor;
    private readonly IImageStorage _imageStorage;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<CreatePostRequestHandler> _logger;
    
    public CreatePostRequestHandler(
        ILogger<CreatePostRequestHandler> logger, 
        ImageProcessor imageProcessor,
        IImageStorage imageStorage,
        ApplicationDbContext dbContext)
    {
        _logger = logger;
        _imageProcessor = imageProcessor;
        _imageStorage = imageStorage;
        _dbContext = dbContext;

        // TODO: We need to ensure that before deployment blob containers exists
    }
    
    public async Task<CreatedPost> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        Stream originalImageStream = request.Image.OpenReadStream();
        await using Stream processedImageStream = _imageProcessor.ProcessImage(originalImageStream);
        
        originalImageStream.Position = 0;
        processedImageStream.Position = 0;

        await EnsureDatabaseIsHealthyAsync(cancellationToken);
        
        (SavedImage originalImage, SavedImage processedImage) 
            = await SaveImagesAsync(request.UserId, originalImageStream, processedImageStream, cancellationToken);
        
        var post = await CreatePostAsync(request, originalImage, processedImage, cancellationToken);

        return new CreatedPost(post.Id);
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
            DateTimeOffset.UtcNow);
        
        await _dbContext.Posts.AddAsync(post, cancellationToken);
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
}