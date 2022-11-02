using MediatR;

namespace Imagegram.Features.Comments.DeleteComment;

// TODO: Ensure that CommentBy and user that is executin this command is the same user

/// <param name="InitiatorId">Id of the user who initiated command</param>
public sealed record DeleteCommentCommand(int PostId, int CommentId, int InitiatorId) : IRequest<Unit>;