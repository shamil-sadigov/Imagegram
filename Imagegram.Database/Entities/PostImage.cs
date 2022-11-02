namespace Imagegram.Database.Entities;

#pragma warning disable CS8618
public sealed class PostImage
{
    public PostImage(int postId, ImageInfo processedImage, ImageInfo originalImage)
    {
        PostId = postId;
        ProcessedImage = processedImage ?? throw new ArgumentNullException(nameof(processedImage));
        OriginalImage = originalImage ?? throw new ArgumentNullException(nameof(originalImage));
    }

    // For EF
    private PostImage()
    {
        
    }
    
    public int PostId { get; }
    
    public ImageInfo ProcessedImage { get; }
    
    public ImageInfo OriginalImage { get; }
}