using Imagegram.Features.Posts;

namespace Imagegram.Features;

public record PostDto
(
    int Id,
    int CommentCount,
    DateTimeOffset LastTimeUpdatedAt,
    DateTimeOffset CreatedAt,
    string Description,
    string ImageUrl,
    IEnumerable<CommentDto> Comments
);