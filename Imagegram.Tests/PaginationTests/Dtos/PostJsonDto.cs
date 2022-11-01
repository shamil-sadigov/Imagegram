namespace Imagegram.Tests.PaginationTests.Dtos;

public record PostJsonDto
{
    public string Description { get; set; }
    public int CommentCount { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public List<CommentJsonDto> Comments { get; set; }
}