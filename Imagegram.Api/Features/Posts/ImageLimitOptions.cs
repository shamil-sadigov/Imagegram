namespace Imagegram.Api.Features.Posts;

public sealed class ImageLimitOptions
{
    public long MaxAllowedLength { get; set; }
    
    public string[] AllowedUploadFormats { get; set; }
    
    public int AllowedSize = 600;
}