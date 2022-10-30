using Imagegram.Database.Models;

namespace Imagegram.Features.Posts.GetPaginated;

public static class CollectionExtensions
{
    public static PaginatedResult<T> ToPaginatedResult<T>(
        this ICollection<T> items,
        bool hasMoreData) where T: IHasId
    {
        if (items.Count == 0)
        {
            return PaginatedResult<T>.Empty;
        }
        
        return new PaginatedResult<T>
        (
            PageSize: items.Count,
            StartCursor: items.Select(x=> x.Id).First(),
            EndCursor: items.Select(x => x.Id).Last(),
            items,
            hasMoreData
        );
    }
    
}