using MediatR;

namespace Imagegram.Api.Features.Posts.CreatePost;


public record CreatePostCommand(int UserId, string Description, IFormFile Image) : IRequest<PostDto>;

