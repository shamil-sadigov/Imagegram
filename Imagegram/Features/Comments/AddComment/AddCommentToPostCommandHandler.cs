using System.Data;
using Imagegram.Database;
using Imagegram.Database.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Imagegram.Features.Comments.AddComment;

public class AddCommentToPostCommandHandler : IRequestHandler<AddCommentToPostCommand, AddedComment>
{
    private readonly ApplicationDbContext _db;

    public AddCommentToPostCommandHandler(ApplicationDbContext db)
    {
        _db = db;
    }
    
    public async Task<AddedComment> Handle(AddCommentToPostCommand request, CancellationToken cancellationToken)
    {
        Comment addedComment = default;
        
        await InTransactionAsync(async () =>
        {
            var post = await FindPostAsync(request, cancellationToken);
            
            addedComment = post.AddComment(request.CommentText, request.CommentedBy, DateTimeOffset.UtcNow);

            await _db.SaveChangesAsync(cancellationToken);

        }, cancellationToken);

        return new AddedComment(addedComment!.Id, addedComment.PostId, addedComment.CommentedBy);
    }

    private async Task<Post> FindPostAsync(AddCommentToPostCommand request, CancellationToken cancellationToken)
    {
        var post = await _db.Posts.FirstOrDefaultAsync(x => x.Id == request.PostId, cancellationToken);
        
        if (post is null)
        {
            throw new EntityNotFoundException($"Post {request.PostId} was not found");
        }
        
        return post;
    }

    private async Task InTransactionAsync(Func<Task> dbOperation,  CancellationToken cancellationToken)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead, cancellationToken);

        try
        {
            await dbOperation();
        
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception e)
        {
            // TODO: Log
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}