using MediatR;

namespace Imagegram.Features.Posts.CreatePost;


public record CreatePostCommand(int UserId, string Description, IFormFile Image) : IRequest<PostDto>;

