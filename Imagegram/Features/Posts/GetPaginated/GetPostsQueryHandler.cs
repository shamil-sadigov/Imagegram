using Imagegram.Database;
using Imagegram.Features.Posts.GetPaginated.Pagination;
using Imagegram.Features.Posts.GetPaginated.PaginationStrategies;
using MediatR;

namespace Imagegram.Features.Posts.GetPaginated;

public class GetPostsQueryHandler : IRequestHandler<GetPostsQuery, GetPostsQueryResult>
{
    private readonly ApplicationDbContext _dbContext;
    
    public GetPostsQueryHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<GetPostsQueryResult> Handle(GetPostsQuery query, CancellationToken cancellationToken)
    {
        PaginatedResult<PostDto, PostCursor> paginatedResult = query switch
        {
            { IsFirstPageRequested: true } 
                => await new FirstPagePaginationStrategy(_dbContext).PaginateAsync(query.PageSize, null),
            
            { IsNextPageRequested: true } 
                 => await new NextPagePaginationStrategy(_dbContext).PaginateAsync(query.PageSize, query.EndCursor),
            
            { IsPreviousPageRequested: true } 
                => await new PreviousPagePaginationStrategy(_dbContext).PaginateAsync(query.PageSize, query.StartCursor),

            _ => throw new InvalidOperationException($"Unexpected query {query}")
        };

        return new GetPostsQueryResult(paginatedResult);
    }
}