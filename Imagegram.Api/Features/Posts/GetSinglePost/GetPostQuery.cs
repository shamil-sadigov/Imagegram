using MediatR;

namespace Imagegram.Api.Features.Posts.GetSinglePost;

public record GetPostQuery(int PostId, bool ShouldIncludeComments) : IRequest<PostDto>;