using Imagegram.Database;
using Microsoft.EntityFrameworkCore;

namespace Imagegram.Features.Posts.GetPaginated.PostPaginationStrategies;

/// <summary>
/// Creates pagination for first page
/// </summary>
public class FirstPagePaginationStrategy:BasePaginationStrategy
{
    private readonly ApplicationDbContext _dbContext;

    public FirstPagePaginationStrategy(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public override async Task<PaginatedResult<PostDto, PostCursor>> PaginateAsync(PageSize pageSize, PostCursor? cursor)
    {
        var prefetchCount = pageSize + 1;
        
        var result = await _dbContext.Posts
            .OrderByDescending(post => post.CommentCount)
            .ThenByDescending(post => post.LastTimeUpdatedAt)
            .ThenByDescending(post => post.Id)
            .Take(prefetchCount)
            .Select(post => ProjectToDto(post))
            .ToListAsync();
        
        return Paginate(pageSize, result, prefetchCount);
    }
    
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
}