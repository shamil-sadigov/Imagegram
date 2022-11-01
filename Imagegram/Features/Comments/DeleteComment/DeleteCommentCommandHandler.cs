using System.Data;
using Imagegram.Database;
using Imagegram.Database.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Imagegram.Features.Comments.DeleteComment;

public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, CommentRemoved>
{
    private readonly ApplicationDbContext _db;

    public DeleteCommentCommandHandler(ApplicationDbContext db) => _db = db;

    public async Task<CommentRemoved> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        await _db.InTransactionAsync(IsolationLevel.RepeatableRead, async () =>
        {
            var post = await FindPostAsync(request, cancellationToken);

            post.RemoveComment(request.CommentId, DateTimeOffset.UtcNow);

            await _db.SaveChangesAsync(cancellationToken);
            
        }, cancellationToken);
        
        return new CommentRemoved();
    }

    private async Task<Post> FindPostAsync(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var post = await _db.Posts
            .Include(x => x.Comments.Where(x => x.CommentedBy == request.CommentedBy && x.Id == request.CommentId))
            .FirstOrDefaultAsync(x => x.Id == request.PostId, cancellationToken: cancellationToken);
        
        if (post is null)
        {
            throw new EntityNotFoundException($"Post {request.PostId} was not found");
        }
        
        return post;
    }
}