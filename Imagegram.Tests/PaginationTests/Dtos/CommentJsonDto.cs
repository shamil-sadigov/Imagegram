namespace Imagegram.Tests.PaginationTests.Dtos;

public record CommentJsonDto
{
    public string Text { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}