namespace Imagegram.Features.Posts.GetPaginated;

public interface IPaginationStrategy<TEntity, TCursor>
{
    Task<PaginatedResult<TEntity, TCursor>> PaginateAsync(PageSize pageSize, TCursor? cursor);
}