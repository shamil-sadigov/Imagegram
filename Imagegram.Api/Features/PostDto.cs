using Imagegram.Api.Features.Posts;

namespace Imagegram.Api.Features;

public record PostDto
(
    int PostId,
    int CreatedBy,
    int CommentCount,
    DateTimeOffset LastTimeUpdatedAt,
    DateTimeOffset CreatedAt,
    string Description,
    string ImageUrl,
    IEnumerable<CommentDto>? Comments
);