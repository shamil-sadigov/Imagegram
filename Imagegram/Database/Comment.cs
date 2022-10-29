
namespace Imagegram.Database;

#pragma warning disable CS8618
public class Comment
{
    public int Id { get; set; }
    
    /// <summary>
    /// User who made a comment
    /// </summary>
    public int UserId { get; set; }
    
    public string Text { get; set; }
    
    public int PostId { get; set; }
}