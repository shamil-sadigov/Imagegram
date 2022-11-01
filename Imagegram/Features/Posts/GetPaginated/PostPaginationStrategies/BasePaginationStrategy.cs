using Imagegram.Database.Models;

namespace Imagegram.Features.Posts.GetPaginated.PostPaginationStrategies;

public abstract class BasePaginationStrategy:IPaginationStrategy<PostDto, PostCursor>
{
    // TODO: What will it return if there is not comments
    // TODO: It may load all comments in memory ?
    
    protected static PostDto ProjectToDto(Post x) =>
        new
        (
            x.Id,
            x.CommentCount, 
            x.LastTimeUpdatedAt, 
            x.CreatedAt, 
            x.Description, 
            x.Image.ProcessedImage.Uri, 
            x.CommentCount < 1 // TODO: Maybe delete it ?
                ? Enumerable.Empty<CommentDto>()
                : x.Comments!.OrderByDescending(c => c.CreatedAt)
                    .Take(2)
                    .Select(c => new CommentDto(c.Id, c.Text, c.CommentedBy)));

    public abstract Task<PaginatedResult<PostDto, PostCursor>> PaginateAsync(PageSize pageSize, PostCursor? cursor);
}