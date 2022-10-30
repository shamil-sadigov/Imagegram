#pragma warning disable CS8618
namespace Imagegram.Database.Models;

public class Post
{
    public long Id { get; set; }
    
    public int OwnerId { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    // TODO: Any limit ?
    public string Description { get; set; }
    
    public PostImage Image { get; set; }
    
    public List<Comment>? Comments { get;  set; }
}