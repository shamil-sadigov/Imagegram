using System.Data;
using Imagegram.Api.Extensions;
using Imagegram.Database;
using Imagegram.Database.Entities;
using MediatR;

namespace Imagegram.Api.Features.Comments.AddComment;

public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, AddedComment>
{
    private readonly ApplicationDbContext _db;
    private readonly ISystemTime _systemTime;

    public AddCommentCommandHandler(ApplicationDbContext db, ISystemTime systemTime)
    {
        _db = db;
        _systemTime = systemTime;
    }
    
    public async Task<AddedComment> Handle(AddCommentCommand command, CancellationToken cancellationToken)
    {
        Comment addedComment = default;
        
        await _db.InTransactionAsync(IsolationLevel.RepeatableRead, async () =>
        {
            var post = await _db.Posts.FindOrThrowAsync(command.PostId, cancellationToken);
            
            addedComment = post.AddComment(command.CommentText, command.CommentedBy, _systemTime.CurrentUtc);

            await _db.SaveChangesAsync(cancellationToken);
            
        }, cancellationToken);
        
        return new AddedComment(addedComment!.Id, addedComment.PostId, addedComment.CommentedBy);
    }
}