using Imagegram.Features.Posts.GetPaginated.Pagination;
using MediatR;

namespace Imagegram.Features.Posts.GetPaginated;

public sealed record GetPostsQuery(PageSize PageSize, PostCursor? BeforeCursor, PostCursor? AfterCursor)
    : IRequest<PaginatedResult<PostDto, PostCursor>>
{
    public bool IsFirstPageRequested => BeforeCursor is null && AfterCursor is null;
    
    public bool IsNextPageRequested => AfterCursor is not null;
    
    public bool IsPreviousPageRequested => BeforeCursor is not null;
}
