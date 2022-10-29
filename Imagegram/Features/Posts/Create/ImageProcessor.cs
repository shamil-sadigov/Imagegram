using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Imagegram.Features.Posts.Create;

public sealed class ImageProcessor
{
    // TODO: What if width is shorter than height ?
    // TODO: Extract to configuration
    private const int allowedSize = 600;
    
    /// <summary>
    /// Processes <see cref="sourceImage"/> by resizing and converting to jpeg.
    /// Processed image is written to <see cref="targetStream"/>
    /// 
    /// </summary>
    public void ProcessImage(Stream sourceImageStream, Stream targetImageStream)
    {
        var image = Image.Load(sourceImageStream);
        
        var height = (allowedSize * image.Height) / image.Width;
        
        image.Mutate(x => x.Resize(allowedSize, height));
        
        image.SaveAsJpeg(targetImageStream);
    }
}

/// <param name="Name">Name of the file thath represent the image</param>
/// <param name="Uri">Uri at which image is available</param>
public record struct SavedImage(string Name, string Uri);