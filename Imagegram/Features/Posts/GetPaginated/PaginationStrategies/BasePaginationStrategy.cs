using Imagegram.Database.Entities;
using Imagegram.Features.Posts.GetPaginated.Pagination;

namespace Imagegram.Features.Posts.GetPaginated.PaginationStrategies;

public abstract class BasePaginationStrategy:IPaginationStrategy<PostDto, PostCursor>
{
    protected static PostDto ProjectToDto(Post x) =>
        new
        (
            x.Id,
            x.CommentCount, 
            x.LastTimeUpdatedAt, 
            x.CreatedAt, 
            x.Description, 
            x.Image.ProcessedImage.Uri, 
            x.CommentCount < 1
                ? Enumerable.Empty<CommentDto>()
                : x.Comments!.OrderByDescending(c => c.CreatedAt)
                    .Take(2)
                    .Select(c => new CommentDto(c.Id, c.Text, c.CommentedBy)));

    public abstract Task<PaginatedResult<PostDto, PostCursor>> PaginateAsync(PageSize pageSize, PostCursor? cursor);
}