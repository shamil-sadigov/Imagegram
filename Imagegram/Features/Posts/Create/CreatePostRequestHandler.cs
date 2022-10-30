using Imagegram.Database;
using Imagegram.Database.Models;
using MediatR;

namespace Imagegram.Features.Posts.Create;

public class CreatePostRequestHandler : IRequestHandler<CreatePostCommand, CreatedPost>
{
    private readonly ImageProcessor _imageProcessor;
    private readonly ImageStorage _imageStorage;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<CreatePostRequestHandler> _logger;


    public CreatePostRequestHandler(
        ILogger<CreatePostRequestHandler> logger, 
        ImageProcessor imageProcessor,
        ImageStorage imageStorage,
        ApplicationDbContext dbContext)
    {
        _logger = logger;
        _imageProcessor = imageProcessor;
        _imageStorage = imageStorage;
        _dbContext = dbContext;

        // TODO: We need to ensure that before deployment blob containers exists
    }
    
    public async Task<CreatedPost> Handle(CreatePostCommand command, CancellationToken cancellationToken)
    {
        Stream originalImageStream = command.Image.OpenReadStream();
        await using var processedImageStream = _imageProcessor.ProcessImage(originalImageStream);
        
        originalImageStream.Position = 0;
        processedImageStream.Position = 0;
        
        (SavedImage originalImage, SavedImage processedImage) 
            = await SaveImagesAsync(command.UserId, originalImageStream, processedImageStream);
        
        var post = await CreatePostAsync(command.Description, originalImage, processedImage, cancellationToken);

        return new CreatedPost(post.Id);
    }

    private async Task<Post> CreatePostAsync(
        string postDescription,
        SavedImage originalImage, 
        SavedImage processedImage,
        CancellationToken cancellationToken)
    {
        var post = new Post()
        {
            Description = postDescription,
            CreatedAt = DateTime.UtcNow, // TODO: Abstract it,
            Image = new PostImage()
            {
                OriginalImage = new ImageInfo(originalImage.Name, originalImage.Uri),
                ProcessedImage = new ImageInfo(processedImage.Name, processedImage.Uri)
            }
        };

        await _dbContext.Posts.AddAsync(post, cancellationToken);
        return post;
    }

    private async Task<(SavedImage, SavedImage)> SaveImagesAsync(
        int userId,
        Stream originalImageStream,
        Stream processedImageStream)
    {
        var imageName = $"{userId}-{Guid.NewGuid()}.jpg";
        
        var savingOriginalImage = _imageStorage.SaveOriginalImageAsync(imageName, originalImageStream);
        var savingProcessedImage = _imageStorage.SaveProcessedImageAsync(imageName, processedImageStream);

        await Task.WhenAll
        (
            savingOriginalImage,
            savingProcessedImage
        );
        
        return (savingOriginalImage.Result, savingProcessedImage.Result);
    }
}