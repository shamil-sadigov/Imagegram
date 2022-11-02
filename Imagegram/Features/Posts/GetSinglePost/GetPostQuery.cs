using MediatR;

namespace Imagegram.Features.Posts.GetSinglePost;

public record GetPostQuery(int PostId, bool ShouldIncludeComments) : IRequest<PostDto>;