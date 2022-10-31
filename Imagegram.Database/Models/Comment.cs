
namespace Imagegram.Database.Models;

#pragma warning disable CS8618
public sealed class Comment:BaseEntity
{
    /// <summary>
    /// User who made a comment
    /// </summary>
    public int UserId { get; init; }
    
    public string Text { get; init; }
    
    public int PostId { get; init; }
}