using System.Data;
using Imagegram.Database;
using Imagegram.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Imagegram.Features.Comments.DeleteComment;

public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, Unit>
{
    private readonly ApplicationDbContext _db;
    private readonly ISystemTime _systemTime;
    
    public DeleteCommentCommandHandler(ApplicationDbContext db, ISystemTime systemTime)
    {
        _db = db;
        _systemTime = systemTime;
    }

    public async Task<Unit> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        await _db.InTransactionAsync(IsolationLevel.RepeatableRead, async () =>
        {
            var post = await _db.Posts
                .Include(x => x.Comments.Where(c => c.Id == request.CommentId))
                .FindOrThrowAsync(request.PostId, cancellationToken);
            
            post.RemoveComment(request.CommentId, request.InitiatorId, _systemTime.CurrentUtc);

            await _db.SaveChangesAsync(cancellationToken);
            
        }, cancellationToken);
        
        return Unit.Value;
    }
}