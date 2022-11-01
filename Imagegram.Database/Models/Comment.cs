
namespace Imagegram.Database.Models;

#pragma warning disable CS8618
public sealed class Comment:BaseEntity
{
    /// <summary>
    /// User who made a comment
    /// </summary>
    public int CommentedBy { get; init; }
    
    public string Text { get; init; }
    
    public int PostId { get; init; }

    public override string ToString()
    {
        return $"CommentedBy: {CommentedBy}, Text: {Text}";
    }
}