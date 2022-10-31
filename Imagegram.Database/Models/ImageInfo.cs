namespace Imagegram.Database.Models;

#pragma warning disable CS8618
public sealed class ImageInfo
{
    public string Name { get; }
    public string Uri { get; }

    public ImageInfo(string name, string uri)
    {
        Name = name;
        Uri = uri;
    }

    // For EF
    private ImageInfo()
    {
        
    }
}