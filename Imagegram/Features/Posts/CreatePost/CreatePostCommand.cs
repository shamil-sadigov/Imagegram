using MediatR;

namespace Imagegram.Features.Posts.CreatePost;


public record CreatePostCommand(int UserId, string Description, IFormFile Image) : IRequest<CreatedPost>;

public record CreatedPost(long PostId);


//
// public sealed class CreatePostCommand:IRequest<CreatedPost>, IValidatableObject
// {
//     public int CommentedBy { get; set; }
//     public string Description { get; set; }
//     public IFormFile Image { get; set; }
//     
//     // TODO: Extract it to separete model
//     public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
//     {
//         // TODO: Validate extensions, and file length
//         // TODO: And length of Description
//         if (Description.Length > 1)
//         {
//             yield return new ValidationResult("Bad baby");
//         }
//     }
// }