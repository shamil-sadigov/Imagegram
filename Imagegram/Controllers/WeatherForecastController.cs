using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;

namespace Imagegram.Controllers;

public class CreatePostRequest:IValidatableObject
{
    public string Description { get; set; }
    public IFormFile Image { get; set; }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // TODO: Validate extensions, and file length
        // TODO: And length of Description
        if (Description.Length > 1)
        {
            yield return new ValidationResult("Bad baby");
        }
    }
}


[ApiController]
[Route("[controller]")]
public class ImagesController : ControllerBase
{
    private readonly ILogger<ImagesController> _logger;
    private readonly ImageProcessor _imageProcessor;
    private readonly PostImageStorage _imageStorage;


    public ImagesController(
        ILogger<ImagesController> logger, 
        ImageProcessor imageProcessor,
        PostImageStorage imageStorage)
    {
        _logger = logger;
        _imageProcessor = imageProcessor;
        _imageStorage = imageStorage;


        // TODO: We need to ensure that before deployment blob containers exists

        // "processed-images"
    }

    // TODO: Think about FromForm, it's tied to web

    [HttpPost]
    [RequestFormLimits(MultipartBodyLengthLimit = 268435456)]
    [RequestSizeLimit(268435456)]
    public async Task<IActionResult> Get([FromForm] CreatePostRequest request)
    {
        var startNew = Stopwatch.StartNew();
        
        var imageName = $"{{UserId}} {Guid.NewGuid()}.jpg";
        
        Stream originalImageStream = request.Image.OpenReadStream();
        await using var processedImageStream = new MemoryStream();

        _imageProcessor.ProcessImage(originalImageStream, processedImageStream);
        
        originalImageStream.Position = 0;
        processedImageStream.Position = 0;
        
         await Task.WhenAll(new Task[]
         {
             _imageStorage.SaveOriginalImageAsync(imageName, originalImageStream),
             _imageStorage.SaveProcessedImageAsync(imageName, processedImageStream),
         });
         
        _logger.LogError($"Uploading took time: {startNew.Elapsed:g}. Image Url => {processedImage.Uri.AbsoluteUri}");
        
        return Ok("Pong");
    }
    
}