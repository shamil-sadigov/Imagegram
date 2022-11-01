using Imagegram.Database;
using Imagegram.Features.Posts.GetPaginated.Pagination;
using Imagegram.Features.Posts.GetPaginated.PostPaginationStrategies;
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
                => await new FirstPagePostPaginationStrategy(_dbContext).PaginateAsync(query.PageSize, null),
            
            { IsNextPageRequested: true } 
                 => await new NextPagePostPaginationStrategy(_dbContext).PaginateAsync(query.PageSize, query.EndCursor),
            
            { IsPreviousPageRequested: true } 
                => await new PreviousPagePostPaginationStrategy(_dbContext).PaginateAsync(query.PageSize, query.StartCursor),

            _ => throw new InvalidOperationException($"Unexpected query {query}")
        };

        return new GetPostsQueryResult(paginatedResult);
    }
}