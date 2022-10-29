using MediatR;

namespace Imagegram.Features.Posts.Create;

public class CreatePostRequestHandler : IRequestHandler<CreatePostRequest, int>
{
    private readonly ImageProcessor _imageProcessor;
    private readonly ImageStorage _imageStorage;
    private readonly ILogger<CreatePostRequestHandler> _logger;


    public CreatePostRequestHandler(
        ILogger<CreatePostRequestHandler> logger, 
        ImageProcessor imageProcessor,
        ImageStorage imageStorage)
    {
        _logger = logger;
        _imageProcessor = imageProcessor;
        _imageStorage = imageStorage;
        
        // TODO: We need to ensure that before deployment blob containers exists
    }
    
    public async Task<int> Handle(CreatePostRequest request, CancellationToken cancellationToken)
    {
        var imageName = $"{{UserId}} {Guid.NewGuid()}.jpg";
        
        Stream originalImageStream = request.Image.OpenReadStream();
        await using var processedImageStream = new MemoryStream();

        _imageProcessor.ProcessImage(originalImageStream, processedImageStream);
        
        originalImageStream.Position = 0;
        processedImageStream.Position = 0;
        
        await Task.WhenAll
        (
            _imageStorage.SaveOriginalImageAsync(imageName, originalImageStream), 
            _imageStorage.SaveProcessedImageAsync(imageName, processedImageStream)
        );

    }
}