using Imagegram.Api.Features.Posts.GetPaginated.Pagination;
using Imagegram.Database;
using Microsoft.EntityFrameworkCore;

namespace Imagegram.Api.Features.Posts.GetPaginated.PaginationStrategies;

/// <summary>
/// Creates pagination for the next page
/// </summary>
public class NextPagePaginationStrategy:BasePaginationStrategy
{
    private readonly ApplicationDbContext _dbContext;

    public NextPagePaginationStrategy(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public override async Task<PaginatedResult<PostDto, PostCursor>> PaginateAsync(PageSize pageSize, PostCursor? cursor)
    {
        var prefetchCount = pageSize + 1;
        
        if (cursor is null)
        {
            throw new ArgumentException(nameof(cursor));
        }

        DateTimeOffset lastUpdatedTime = new DateTimeOffset(cursor.Timestamp, TimeSpan.Zero);
        
        var result = await _dbContext.Posts
            .OrderByDescending(post => post.CommentCount)
            .ThenByDescending(post => post.LastTimeUpdatedAt)
            .ThenByDescending(post => post.Id)
            .Include(x=> x.Comments)

            // If there are posts with the same 'CommentCount' and same 'LastTimeUpdatedAt' 
            // then return only those which Id < cursor.PostId
            // because posts with Id >= cursor.PostId were already served in previous page

            .Where(post =>
                post.CommentCount < cursor.CommentCount
                || post.CommentCount == cursor.CommentCount 
                && (post.LastTimeUpdatedAt < lastUpdatedTime 
                    || (post.LastTimeUpdatedAt == lastUpdatedTime && post.Id < cursor.PostId)))
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