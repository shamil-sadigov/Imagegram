using Imagegram.Database;
using Imagegram.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Imagegram.Features.Posts.GetPaginated.PostPaginationStrategies;

/// <summary>
/// Creates pagination for first page
/// </summary>
public class NextPagePaginationStrategy:BasePaginationStrategy
{
    private readonly ApplicationDbContext _dbContext;

    public NextPagePaginationStrategy(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    protected override Task<List<PostDto>> RetrievePostDtosAsync(int count, PostCursor? cursor)
    {
        if (cursor is null)
        {
            throw new ArgumentException(nameof(cursor));
        }

        DateTimeOffset lastUpdatedTime = new DateTimeOffset(cursor.Timestamp, TimeSpan.Zero);
        
        return  _dbContext.Posts
            .OrderByDescending(post => post.CommentCount)
               .ThenByDescending(post => post.LastTimeUpdatedAt)
                   .ThenByDescending(post => post.Id)

            // If there are posts with the same 'CommentCount' and same 'LastTimeUpdatedAt' 
            // then return only those which Id < cursor.PostId
            // because posts with Id >= cursor.PostId were already served in previous page

            .Where(post =>
                post.CommentCount < cursor.CommentCount
                || post.CommentCount == cursor.CommentCount 
                && (post.LastTimeUpdatedAt < lastUpdatedTime 
                    || (post.LastTimeUpdatedAt == lastUpdatedTime && post.Id < cursor.PostId)))
            .Take(count)
            .Select(post => ProjectToDto(post))
            .ToListAsync();
    }
}