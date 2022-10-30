using MediatR;

namespace Imagegram.Features.Posts.GetPaginated;

public struct PageSize
{
    public const int MaxAllowedPageSize = 50;
    public const int MinAllowedPageSize = 1;
    
    public int Value { get; }

    public PageSize(int value)
    {
        if (value < MinAllowedPageSize)
            throw new ArgumentOutOfRangeException(nameof(value));
        
        Value = Math.Min(value, MaxAllowedPageSize);
    }
    
    public static implicit operator int (PageSize pageSize)
    {
        return pageSize.Value;
    }
}

public sealed record GetPostsQuery(PageSize PageSize, int? BeforeCursor, int? AfterCursor)
    : IRequest<GetPostsQueryResult>
{
    public bool IsFirstPageRequested => BeforeCursor is null && AfterCursor is null;
    
    public bool IsNextPageRequested => AfterCursor is not null;
    
    public bool IsPreviousPageRequested => BeforeCursor is not null;
}

public sealed record GetPostsQueryResult(PaginatedResult<PostDto> Posts);