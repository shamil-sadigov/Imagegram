using Imagegram.Api.Features.Posts.GetPaginated.Pagination;

namespace Imagegram.Api.Features.Posts.GetPaginated.PaginationStrategies;

public interface IPaginationStrategy<TEntity, TCursor>
{
    Task<PaginatedResult<TEntity, TCursor>> PaginateAsync(PageSize pageSize, TCursor? cursor);
}