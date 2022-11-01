using Imagegram.Database.Models;

namespace Imagegram.Features.Posts.GetPaginated.PostPaginationStrategies;

public abstract class BasePaginationStrategy:IPaginationStrategy<PostDto, PostCursor>
{
    public async Task<PaginatedResult<PostDto, PostCursor>> PaginateAsync(PageSize pageSize, PostCursor? cursor)
    {
        var prefetchCount = pageSize + 1;
        
        var result = await RetrievePostDtosAsync(prefetchCount, cursor);
        
        return Paginate(pageSize, result, prefetchCount);
    }

    protected abstract Task<List<PostDto>> RetrievePostDtosAsync(int count, PostCursor? cursor);
    
    private static PaginatedResult<PostDto, PostCursor> Paginate(int pageSize, List<PostDto> posts, int prefetchCount)
    {
        var hasMoreItems = posts.Count == prefetchCount;

        if (hasMoreItems)
        {
            // Remove last item, because it was extra data fetched from DB
            // to figure out if there are more data or not
            posts.RemoveAt(posts.Count - 1);
        }

        return posts.ToPaginatedResult(pageSize, hasMoreItems);
    }
    
    // TODO: What will it return if there is not comments
    // TODO: It may load all comments in memory ?
    
    protected static PostDto ProjectToDto(Post x) =>
        new
        (
            x.Id,
            x.CommentCount, 
            x.LastTimeUpdatedAt, 
            x.CreatedAt, 
            x.Description, 
            x.Image.ProcessedImage.Uri, 
            x.CommentCount < 1 // TODO: Maybe delete it ?
                ? Enumerable.Empty<CommentDto>()
                : x.Comments!.OrderByDescending(c => c.CreatedAt)
                    .Take(2)
                    .Select(c => new CommentDto(c.Id, c.Text, c.CommentedBy)));
}