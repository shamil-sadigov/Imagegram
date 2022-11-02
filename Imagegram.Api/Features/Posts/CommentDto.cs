namespace Imagegram.Api.Features.Posts;

public record CommentDto(int Id, string Text, int CommentedBy);