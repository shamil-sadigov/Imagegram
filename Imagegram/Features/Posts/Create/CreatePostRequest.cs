using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Imagegram.Features.Posts.Create;

public class CreatePostRequest:IRequest<int>, IValidatableObject
{
    public string Description { get; set; }
    public IFormFile Image { get; set; }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // TODO: Validate extensions, and file length
        // TODO: And length of Description
        if (Description.Length > 1)
        {
            yield return new ValidationResult("Bad baby");
        }
    }
}