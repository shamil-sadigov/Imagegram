namespace Imagegram.Database.Models;

#pragma warning disable CS8618
public sealed class PostImage
{
    public int PostId { get; set; }
    
    public ImageInfo ProcessedImage { get; init; }
    
    public ImageInfo OriginalImage { get; init; }
}