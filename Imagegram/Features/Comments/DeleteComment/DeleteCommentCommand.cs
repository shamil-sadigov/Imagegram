using MediatR;

namespace Imagegram.Features.Comments.DeleteComment;

// TODO: Ensure that CommentBy and user that is executin this command is the same user
public sealed record DeleteCommentCommand(int PostId, int CommentId, int CommentedBy) : IRequest<CommentRemoved>;

public sealed record CommentRemoved();