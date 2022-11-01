using MediatR;

namespace Imagegram.Features.Comments.AddComment;

public sealed record AddCommentToPostCommand(int PostId, int CommentedBy, string CommentText) : IRequest<AddedComment>;
public sealed record AddedComment(int CommentId, int PostId, int commentedBy);



