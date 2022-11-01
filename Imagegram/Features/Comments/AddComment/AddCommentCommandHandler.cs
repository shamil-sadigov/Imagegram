using System.Data;
using Imagegram.Database;
using Imagegram.Database.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Imagegram.Features.Comments.AddComment;

public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, AddedComment>
{
    private readonly ApplicationDbContext _db;

    public AddCommentCommandHandler(ApplicationDbContext db)
    {
        _db = db;
    }
    
    public async Task<AddedComment> Handle(AddCommentCommand request, CancellationToken cancellationToken)
    {
        Comment addedComment = default;
        
        await _db.InTransactionAsync(IsolationLevel.RepeatableRead, async () =>
        {
            var post = await FindPostAsync(request, cancellationToken);
            
            addedComment = post.AddComment(request.CommentText, request.CommentedBy, DateTimeOffset.UtcNow);

            await _db.SaveChangesAsync(cancellationToken);
            
        }, cancellationToken);
        
        return new AddedComment(addedComment!.Id, addedComment.PostId, addedComment.CommentedBy);
    }

    private async Task<Post> FindPostAsync(AddCommentCommand request, CancellationToken cancellationToken)
    {
        var post = await _db.Posts.FirstOrDefaultAsync(x => x.Id == request.PostId, cancellationToken);
        
        if (post is null)
        {
            throw new EntityNotFoundException($"Post {request.PostId} was not found");
        }
        
        return post;
    }
}