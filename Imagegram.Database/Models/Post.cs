using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618
namespace Imagegram.Database.Models;

public sealed class Post:BaseEntity
{
    private List<Comment> _comments =  new(0);
    
    public int OwnerId { get; }
    public string Description { get; }
    public PostImage Image { get; }
    
    // TODO: Add index here
    public int CommentCount { get; private set; }

    public byte[] RowVersion { get; private set; }
    
    public DateTimeOffset? UpdatedAt { get; private set; }
    
    public IReadOnlyCollection<Comment> Comments => _comments;
    
    public Post(
        int ownerId,
        string description,
        ImageInfo originalImage,
        ImageInfo processedImage,
        DateTimeOffset currentDateTime)
    {
        // TODO: Validate correctly Description, ownerId
        
        OwnerId = ownerId;
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Image = new PostImage()
        {
            OriginalImage = originalImage ?? throw new ArgumentNullException(nameof(originalImage)),
            ProcessedImage = processedImage ?? throw new ArgumentNullException(nameof(processedImage))
        };
        CreatedAt = currentDateTime;
    }
    
    // For Ef
    private Post()
    {
        
    }
    
    public void AddNewComment(string comment, int userId, DateTimeOffset currentDateTime)
    {
        _comments!.Add(new Comment()
        {
            PostId = Id,
            Text = comment,
            UserId = userId,
            CreatedAt = currentDateTime
        });
        
        CommentCount++;
        UpdatedAt = currentDateTime;
    }
    
    public void RemoveComment(int commentId, DateTimeOffset currentDateTime)
    {
        if (Comments is null)
        {
            throw new InvalidOperationException("Post has no any comments");
        }
        
        var commentToDelete = Comments.FirstOrDefault(x=> x.Id == commentId);

        if (commentToDelete is null)
        {
            throw new InvalidOperationException($"Comment with id '{commentId}' was not found");
        }

        _comments!.Remove(commentToDelete);
        
        CommentCount--;
        UpdatedAt = currentDateTime;
    }
    
}