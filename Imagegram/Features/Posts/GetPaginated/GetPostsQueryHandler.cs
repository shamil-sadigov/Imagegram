using Imagegram.Database;
using Imagegram.Database.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Imagegram.Features.Posts.GetPaginated;

public class GetPostsQueryHandler : IRequestHandler<GetPostsQuery, GetPostsQueryResult>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<GetPostsQueryHandler> _logger;
    public static readonly Comparer<PostDto> ByCommentCount
        = Comparer<PostDto>.Create((x, y) => x.CommentCount.CompareTo(y.CommentCount));

    public GetPostsQueryHandler(
        ILogger<GetPostsQueryHandler> logger,
        ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
    
    public async Task<GetPostsQueryResult> Handle(GetPostsQuery query, CancellationToken cancellationToken)
    {
        PaginatedResult<PostDto> paginatedResult = query switch
        {
            { IsFirstPageRequested: true } 
                => await CreateFirstPageAsync(query, cancellationToken),
            { IsNextPageRequested: true } 
                => await MoveToNextPageAsync(query, cancellationToken),
            { IsPreviousPageRequested: true } 
                => await MoveToPreviousPageAsync(query, cancellationToken),
            
            _ => throw new InvalidOperationException($"Unexpected query {query}")
        };

        return new GetPostsQueryResult(paginatedResult);
    }

    private async Task<PaginatedResult<PostDto>> CreateFirstPageAsync(GetPostsQuery query,
        CancellationToken cancellationToken)
    {
        var prefetchCount = query.PageSize + 1;

        List<PostDto> posts = await _dbContext.Posts
            .OrderBy(post => post.Id)
            .Take(prefetchCount)
            .Select(post => ProjectToDto(post))
            .ToListAsync(cancellationToken);

        return Paginate(posts, prefetchCount);
    }
    
    private async Task<PaginatedResult<PostDto>> MoveToNextPageAsync(GetPostsQuery query,
        CancellationToken cancellationToken)
    {
        var prefetchCount = query.PageSize + 1;

        List<PostDto> posts = await _dbContext.Posts
            .Where(x => x.Id > query.AfterCursor!.Value)
            .OrderBy(x => x.Id)
            .Take(prefetchCount)
            .Select(post => ProjectToDto(post))
            .ToListAsync(cancellationToken);

        return Paginate(posts, prefetchCount);
    }
    
    private async Task<PaginatedResult<PostDto>> MoveToPreviousPageAsync(GetPostsQuery query,
        CancellationToken cancellationToken)
    {
        var prefetchCount = query.PageSize + 1;

        List<PostDto> posts = await _dbContext.Posts
            .Where(x => x.Id < query.BeforeCursor!.Value)
            .OrderByDescending(x => x.Id)
            .Take(prefetchCount)
            .Select(post => ProjectToDto(post))
            .ToListAsync(cancellationToken);
        
        return Paginate(posts, prefetchCount);
    }

    private static PaginatedResult<PostDto> Paginate(List<PostDto> posts, int prefetchCount)
    {
        var hasMoreData = posts.Count == prefetchCount;

        if (hasMoreData)
        {
            posts.RemoveAt(posts.Count - 1);
        }

        posts.Sort(ByCommentCount);
        return posts.ToPaginatedResult(hasMoreData);
    }

    private static PostDto ProjectToDto(Post x)
    {
        return new PostDto
        (
            x.Id,
            x.Description,
            x.Image.ProcessedImage.Uri,
            x.CommentCount,
            // TODO: Check for null reference
            x.Comments.OrderBy(c => c.CreatedAt).TakeLast(2).Select(c => new CommentDto(c.Id, c.Text, c.UserId)
            ));
    }
}