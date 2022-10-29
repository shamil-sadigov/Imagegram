#pragma warning disable CS8618
namespace Imagegram.Database;

public class Post
{
    public int Id { get; set; }
    
    public int OwnerId { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    // TODO: Any limit ?
    public string Description { get; set; }
    
    public string ImageUrl { get; set; }
    
    public List<Comment> Comments { get;  set; }
}