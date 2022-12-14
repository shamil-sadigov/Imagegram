using Imagegram.Api.Features;
using Imagegram.Api.Features.Comments.AddComment;
using Imagegram.Api.Features.Comments.DeleteComment;
using Imagegram.Api.Features.Posts.CreatePost;
using Imagegram.Api.Features.Posts.GetPaginated;
using Imagegram.Api.Features.Posts.GetPaginated.Pagination;
using Imagegram.Api.Features.Posts.GetSinglePost;
using Imagegram.Api.Requests;
using Imagegram.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Imagegram.Api.Controllers;

// TODO: Add metrics in action filter to log warning whenever requested took longer than 50ms

[Authorize]
[ApiController]
[Route("api/v1/posts")]
public class PostsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PostsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet]
    public async Task<PaginatedPostsResponse> GetPaginatedPosts([FromQuery] GetPostsRequest request, CancellationToken token)
    {
        (PageSize pageSize, PostCursor? beforeCursor, PostCursor? afterCursor) = BuildQueryParameters(request);

        var query = new GetPostsQuery(pageSize, beforeCursor, afterCursor);

        var paginatedResult = await _mediator.Send(query, token);
        
        var response = new PaginatedPostsResponse
        (
            paginatedResult.RequestedPageSize,
            paginatedResult.ActualPageSize,
            paginatedResult.HasMoreItems,
            paginatedResult.Items,
            new CursorsResponse(
                paginatedResult.StartCursor?.UrlEncoded(), 
                paginatedResult.EndCursor?.UrlEncoded())
        );
        
        return response;
    }
    
    /// <param name="includeComments">If 'true' then post will be returned along with comments</param>
    /// <returns></returns>
    [HttpGet("{postId:int}")]
    public async Task<PostDto> GetPostById(int postId, bool includeComments, CancellationToken token)
    {
        var post = await _mediator.Send(new GetPostQuery(postId, includeComments), token);
        return post;
    }
    
    [HttpPost]
    [RequestSizeLimit(105857600)]
    public async Task<IActionResult> CreatePost([FromForm] CreatePostRequest request, CancellationToken token)
    {
        var currentUserId = User.GetId();

        var createdPost = await _mediator.Send(
            new CreatePostCommand(currentUserId, request.Description, request.ImageFile), token);
        
        return CreatedAtAction(nameof(GetPostById), new
        {
            postId = createdPost.PostId
        }, createdPost);
    }
    
    [HttpPost("{postId:int}/comments")]
    public async Task<AddedComment> AddComment(
        int postId, 
        [FromBody] AddCommentRequest request, 
        CancellationToken token)
    {
        var addedComment = await _mediator.Send(
            new AddCommentCommand(
                postId, 
                CommentedBy: User.GetId(),
                request.CommentText), token);

        // Maybe 201 is better
        return addedComment;
    }
    
    [HttpDelete("{postId:int}/comments/{commentId:int}")]
    public async Task<IActionResult> DeleteComment(int postId, int commentId, CancellationToken token)
    {
        await _mediator.Send(
            new DeleteCommentCommand(
                postId,
                commentId,
                InitiatorId: User.GetId()), token);
        
        return NoContent();
    }
    
    private static (PageSize pageSize, PostCursor? beforeCursor, PostCursor? afterCursor) BuildQueryParameters(
        GetPostsRequest request)
    {
        // Only one of the cursor is specified, or non of them.
        
        var pageSize = new PageSize(request.Limit);
        
        if (!string.IsNullOrWhiteSpace(request.BeforeCursor) )
        {
            PostCursor.TryCreateFromUrlEncoded(request.BeforeCursor, out var beforeCursor);
            return (pageSize, beforeCursor, null);
        }
        
        if (!string.IsNullOrWhiteSpace(request.AfterCursor))
        {
            PostCursor.TryCreateFromUrlEncoded(request.AfterCursor, out var afterCursor);
            return (pageSize, null, afterCursor);
        }

        return (pageSize, null, null);
    }

}