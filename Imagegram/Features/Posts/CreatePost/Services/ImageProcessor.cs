using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Imagegram.Features.Posts.CreatePost.Services;

public sealed class ImageProcessor
{
    private readonly ImageLimitOptions _imageLimitOptions;
    
    public ImageProcessor(IOptions<ImageLimitOptions> imageLimitOptions)
    {
        _imageLimitOptions = imageLimitOptions.Value;
    }
    
    /// <summary>
    /// Processes image <see cref="sourceImageStream"/> by resizing and converting to jpeg.
    /// </summary>
    /// <returns>Processed image</returns>
    public Stream ProcessImage(Stream sourceImageStream)
    {
        var image = Image.Load(sourceImageStream);
        
        ResizeImageWithoutPreservingAspectRation(image);

        var processedImageStream = new MemoryStream();
        image.SaveAsJpeg(processedImageStream);
        return processedImageStream;
    }
    
    
    /// <summary>
    /// This option implement requirement of TA but it doesnt preserver image aspect ration
    /// which can make rectangular-shaped image become ugly after resizing
    /// </summary>
    private void ResizeImageWithoutPreservingAspectRation(Image image)
    {
        var allowedSideLength = _imageLimitOptions.AllowedSize;

        image.Mutate(x => x.Resize(width: allowedSideLength, height: allowedSideLength));
    }
    
    /// <summary>
    /// This option doesn't comply with requirement of TA but it's more correct
    /// because aspect ration of image is preserved which prevent
    /// rectangular-shaped image to become ugly after resizing
    /// </summary>
    private  void ResizeImagePreservingAspectRation(Image image)
    {
        var allowedWidthLength = _imageLimitOptions.AllowedSize;
        
        var height = (allowedWidthLength * image.Height) / image.Width;

        image.Mutate(x => x.Resize(allowedWidthLength, height));
    }
}

/// <param name="Name">Name of the file thath represent the image</param>
/// <param name="Uri">Uri at which image is available</param>
public record struct SavedImage(string Name, string Uri);