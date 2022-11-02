using MediatR;

namespace Imagegram.Features.Comments.DeleteComment;

/// <param name="InitiatorId">Id of the user who initiated command</param>
public sealed record DeleteCommentCommand(int PostId, int CommentId, int InitiatorId) : IRequest<Unit>;