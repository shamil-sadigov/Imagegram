using Imagegram.Database.Entities;
using Imagegram.Features.Posts.GetPaginated.Pagination;

namespace Imagegram.Features.Posts.GetPaginated.PaginationStrategies;

public abstract class BasePaginationStrategy:IPaginationStrategy<PostDto, PostCursor>
{
    protected static PostDto ProjectToDto(Post post) =>
        new
        (
            post.Id,
            post.CreatedBy,
            post.CommentCount, 
            post.LastTimeUpdatedAt, 
            post.CreatedAt, 
            post.Description, 
            post.Image.ProcessedImage.Uri, 
            post.CommentCount < 1
                ? null
                : post.Comments!.OrderByDescending(c => c.CreatedAt)
                    .Take(2)
                    .Select(c => new CommentDto(c.Id, c.Text, c.CommentedBy)));

    public abstract Task<PaginatedResult<PostDto, PostCursor>> PaginateAsync(PageSize pageSize, PostCursor? cursor);
}