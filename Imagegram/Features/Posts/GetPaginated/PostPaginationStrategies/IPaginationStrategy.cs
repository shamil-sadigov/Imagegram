using Imagegram.Features.Posts.GetPaginated.Pagination;

namespace Imagegram.Features.Posts.GetPaginated.PostPaginationStrategies;

public interface IPaginationStrategy<TEntity, TCursor>
{
    Task<PaginatedResult<TEntity, TCursor>> PaginateAsync(PageSize pageSize, TCursor? cursor);
}