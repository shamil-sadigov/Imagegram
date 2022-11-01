namespace Imagegram.Features.Posts.GetPaginated;

public record PaginatedResult<T, TCursor>
(
    int RequestedPageSize,
    int ActualPageSize,
    TCursor? StartCursor,
    TCursor? EndCursor,
    IEnumerable<T> Items,
    bool HasMoreItems
)
{
    public static readonly PaginatedResult<T, TCursor> Empty = new(0, 0, default, default, Array.Empty<T>(), false);
}
