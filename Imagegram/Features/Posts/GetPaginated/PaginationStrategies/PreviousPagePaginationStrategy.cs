using Imagegram.Database;
using Imagegram.Features.Posts.GetPaginated.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Imagegram.Features.Posts.GetPaginated.PaginationStrategies;

/// <summary>
/// Creates pagination for the previous pages
/// </summary>
public class PreviousPagePaginationStrategy:BasePaginationStrategy
{
    private readonly ApplicationDbContext _dbContext;

    public PreviousPagePaginationStrategy(ApplicationDbContext dbContext)
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
        
        var posts = await  _dbContext.Posts
            .OrderBy(post => post.CommentCount)
            .ThenBy(post => post.LastTimeUpdatedAt)
            .ThenBy(post => post.Id)

            // TODO: Change this comment
            // If there are posts with the same 'CommentCount' and same 'LastTimeUpdatedAt' 
            // then return only those which Id < cursor.PostId
            // because posts with Id >= cursor.PostId were already served in previous page

            .Where(post =>
                post.CommentCount > cursor.CommentCount
                || post.CommentCount == cursor.CommentCount 
                && (post.LastTimeUpdatedAt > lastUpdatedTime 
                    || (post.LastTimeUpdatedAt == lastUpdatedTime && post.Id > cursor.PostId)))
            .Take(prefetchCount)
            .Select(post => ProjectToDto(post))
            .ToArrayAsync();
        
        var ordered =  posts.OrderByDescending(x => x.CommentCount)
            .ThenByDescending(x => x.LastTimeUpdatedAt)
            .ThenByDescending(x => x.Id)
            .ToList();

        return Paginate(pageSize, ordered, prefetchCount);
    }
    
    private static PaginatedResult<PostDto, PostCursor> Paginate(int pageSize, List<PostDto> posts, int prefetchCount)
    {
        var hasMoreItems = posts.Count == prefetchCount;

        if (hasMoreItems)
        {
            // Remove first item, because it was extra data fetched from DB
            // to figure out if there are more data or not
            posts.RemoveAt(0);
        }

        return posts.ToPaginatedResult(pageSize, hasMoreItems);
    }
}