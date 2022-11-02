namespace Imagegram.Features.Posts.GetPaginated.Pagination;

public static class CollectionPaginationExtensions
{
    public static PaginatedResult<PostDto, PostCursor> ToPaginatedResult(
        this ICollection<PostDto> posts,
        int pageSize,
        bool hasMoreItems)
    {
        if (posts.Count == 0)
        {
            return PaginatedResult<PostDto, PostCursor>.Empty;
        }
        
        var firstPost = posts.First();
        var endCursor = posts.Last();
        
        return new PaginatedResult<PostDto, PostCursor>(
            RequestedPageSize: pageSize,
            ActualPageSize: posts.Count,
            StartCursor: PostCursor.FromPost(firstPost), 
            EndCursor: PostCursor.FromPost(endCursor), 
            Items: posts, 
            hasMoreItems);
    }
    
}