namespace Imagegram.Features.Posts.GetPaginated;

public record CommentDto(int Id, string Text, int UserId): IHasId;