using Imagegram.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Imagegram.Features.Posts.GetSinglePost;

// TODO: Add logging everywhere

public class GetPostQyeryHandler : IRequestHandler<GetPostQuery, PostDto>
{
    private readonly ApplicationDbContext _db;


    public GetPostQyeryHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<PostDto> Handle(GetPostQuery request, CancellationToken cancellationToken)
    {
        PostDto? post;
        
        if (request.ShouldIncludeComments)
        {
            post = await _db.Posts
                .Where(x=> x.Id == request.PostId)
                .Select(x => new PostDto
                (
                    x.Id,
                    x.CreatedBy,
                    x.CommentCount,
                    x.LastTimeUpdatedAt,
                    x.CreatedAt,
                    x.Description,
                    x.Image.ProcessedImage.Uri,
                    x.Comments.Select(c => new CommentDto(c.Id, c.Text, c.CommentedBy))))
                .FirstOrDefaultAsync(cancellationToken);
        }
        else
        {
            post = await _db.Posts
                .Where(x=> x.Id == request.PostId)
                .Select(x => new PostDto
                (
                    x.Id,
                    x.CreatedBy,
                    x.CommentCount,
                    x.LastTimeUpdatedAt,
                    x.CreatedAt,
                    x.Description,
                    x.Image.ProcessedImage.Uri,
                    null))
                .FirstOrDefaultAsync(cancellationToken);
        }
   
        
        if (post is null)
        {
            throw new EntityNotFoundException($"Post was not found by Id '{request.PostId}'");
        }

        return post;
    }
    
}