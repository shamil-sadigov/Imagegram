using Imagegram.Features.Posts.GetPaginated.Pagination;

namespace Imagegram.Features.Posts.GetPaginated.PaginationStrategies;

public interface IPaginationStrategy<TEntity, TCursor>
{
    Task<PaginatedResult<TEntity, TCursor>> PaginateAsync(PageSize pageSize, TCursor? cursor);
}