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
    
    protected override Task<List<PostDto>> RetrievePostDtosAsync(int count, PostCursor? cursor)
    {
        return  _dbContext.Posts
            .OrderByDescending(post => post.CommentCount)
                .ThenByDescending(post => post.LastTimeUpdatedAt)
                    .ThenByDescending(post => post.Id)
            .Take(count)
            .Select(post => ProjectToDto(post))
            .ToListAsync();
    }
}