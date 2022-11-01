﻿#pragma warning disable CS8618
namespace Imagegram.Database.Models;

public sealed class Post:BaseEntity
{
    private List<Comment> _comments =  new(0);
    
    /// <summary>
    /// User who created post
    /// </summary>
    public int CreatedBy { get; }
    public string Description { get; }
    public PostImage Image { get; }
    
    // TODO: Add index here
    public int CommentCount { get; private set; }

    public byte[] RowVersion { get; private set; }
    
    public DateTimeOffset LastTimeUpdatedAt { get; private set; }
    
    public IReadOnlyCollection<Comment> Comments => _comments;
    
    /// <param name="createdBy">commentedBy of user who creates a post</param>
    public Post(
        int createdBy,
        string description,
        ImageInfo originalImage,
        ImageInfo processedImage,
        DateTimeOffset currentDateTime)
    {
        // TODO: Validate correctly Description, createdBy
        
        CreatedBy = createdBy;
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Image = new PostImage(Id, processedImage, originalImage);
        CreatedAt = currentDateTime;
        LastTimeUpdatedAt = currentDateTime;
    }
    
    // For Ef
    private Post()
    {
        
    }
    
    public Comment AddComment(string comment, int commentedBy, DateTimeOffset currentDateTime)
    {
        var newComment = new Comment()
        {
            PostId = Id,
            Text = comment,
            CommentedBy = commentedBy,
            CreatedAt = currentDateTime
        };
        
        _comments!.Add(newComment);
        
        CommentCount++;
        LastTimeUpdatedAt = currentDateTime;

        return newComment;
    }
    
    public void RemoveComment(int commentId, DateTimeOffset currentDateTime)
    {
        if (_comments is null)
        {
            throw new InvalidOperationException("Post has no any comments");
        }
        
        var commentToDelete = _comments.FirstOrDefault(x=> x.Id == commentId);

        if (commentToDelete is null)
        {
            throw new InvalidOperationException($"AddComment with id '{commentId}' was not found");
        }

        _comments.Remove(commentToDelete);
        
        CommentCount--;
        LastTimeUpdatedAt = currentDateTime;
    }

    public override string ToString()
    {
        return $"ID: {Id}, CommentCount: {CommentCount}, LastUpdate: {LastTimeUpdatedAt}";
    }
}