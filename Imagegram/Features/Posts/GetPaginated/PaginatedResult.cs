namespace Imagegram.Features.Posts.GetPaginated;

public sealed record PaginatedResult<T>
(
    int PageSize,
    int? StartCursor,
    int? EndCursor,
    IEnumerable<T> Items,
    bool HasMore
)
{
    public static readonly PaginatedResult<T> Empty = new(0, null, null, Array.Empty<T>(), false);
}