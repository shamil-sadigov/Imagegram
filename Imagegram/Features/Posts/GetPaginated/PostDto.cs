namespace Imagegram.Features.Posts.GetPaginated;

public record PostDto
(
    int Id,
    string Description,
    string ImageUrl,
    int CommentCount,
    IEnumerable<CommentDto> Comments
) : IHasId;