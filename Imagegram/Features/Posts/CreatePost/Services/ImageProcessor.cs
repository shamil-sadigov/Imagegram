using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Imagegram.Features.Posts.CreatePost.Services;

public sealed class ImageProcessor
{
    // TODO: What if width is shorter than height ?
    // TODO: Extract to configuration
    private const int AllowedSize = 600;
    
    /// <summary>
    /// Processes <see cref="sourceImage"/> by resizing and converting to jpeg.
    /// Processed image is written to <see cref="targetStream"/>
    /// 
    /// </summary>
    public Stream ProcessImage(Stream sourceImageStream)
    {
        var image = Image.Load(sourceImageStream);
        
        var height = (AllowedSize * image.Height) / image.Width;
        
        image.Mutate(x => x.Resize(AllowedSize, height));
        
        var processedImageStream = new MemoryStream();
        image.SaveAsJpeg(processedImageStream);
        return processedImageStream;
    }
}

/// <param name="Name">Name of the file thath represent the image</param>
/// <param name="Uri">Uri at which image is available</param>
public record struct SavedImage(string Name, string Uri);