namespace Imagegram.Database.Models;

#pragma warning disable CS8618
public class PostImage
{
    public long Id { get; set; }
    public long PostId { get; set; }
    
    public ImageInfo ProcessedImage { get; set; }
    
    public ImageInfo OriginalImage { get; set; }
}

public class ImageInfo
{
    public string Name { get;  }
    public string Uri { get; }

    public ImageInfo(string Name, string Uri)
    {
        this.Name = Name;
        this.Uri = Uri;
    }

    // For EF
    private ImageInfo()
    {
        
    }
}