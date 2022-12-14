using System.Data;
using Imagegram.Api.Extensions;
using Imagegram.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Imagegram.Api.Features.Comments.DeleteComment;

public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, Unit>
{
    private readonly ApplicationDbContext _db;
    private readonly ISystemTime _systemTime;
    
    public DeleteCommentCommandHandler(ApplicationDbContext db, ISystemTime systemTime)
    {
        _db = db;
        _systemTime = systemTime;
    }

    public async Task<Unit> Handle(DeleteCommentCommand command, CancellationToken cancellationToken)
    {
        // TODO: Maybe it's better to SELECT FOR UPDATE instead of REPETABLE_READ isoliation level

        await _db.InTransactionAsync(IsolationLevel.RepeatableRead, async () =>
        {
            var post = await _db.Posts
                .Include(x => x.Comments.Where(c => c.Id == command.CommentId))
                .FindOrThrowAsync(command.PostId, cancellationToken);
            
            post.RemoveComment(command.CommentId, command.InitiatorId, _systemTime.CurrentUtc);

            await _db.SaveChangesAsync(cancellationToken);
            
        }, cancellationToken);
        
        return Unit.Value;
    }
}