using Imagegram.Features.Posts.GetPaginated.Pagination;
using MediatR;

namespace Imagegram.Features.Posts.GetPaginated;

public sealed record GetPostsQuery(PageSize PageSize, PostCursor? StartCursor, PostCursor? EndCursor)
    : IRequest<GetPostsQueryResult>
{
    public bool IsFirstPageRequested => StartCursor is null && EndCursor is null;
    
    public bool IsNextPageRequested => EndCursor is not null;
    
    public bool IsPreviousPageRequested => StartCursor is not null;
}

public sealed record GetPostsQueryResult(PaginatedResult<PostDto, PostCursor> Posts);