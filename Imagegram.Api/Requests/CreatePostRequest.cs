using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Imagegram.Api.Features.Posts;
using Microsoft.Extensions.Options;
using MoreLinq.Extensions;

#pragma warning disable CS8618

namespace Imagegram.Api.Requests;

[SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract")]

public sealed class CreatePostRequest : IValidatableObject
{
    [Required]
    [MinLength(1)]
    public string Description { get; set; }
    
    [Required]
    public IFormFile ImageFile { get; set; }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var imageLimitOptions = validationContext.GetRequiredService<IOptions<ImageLimitOptions>>().Value;
        
        if (ImageFile.Length > imageLimitOptions.MaxAllowedLength)
        {
            yield return new ValidationResult(
                $"ImageFile length cannot be greater than {imageLimitOptions.MaxAllowedLength}");
        }
        
        var imageFormat = Path
            .GetExtension(ImageFile.FileName) // .jpg
            .TrimStart('.'); // jpg
        
        if (!imageLimitOptions.AllowedUploadFormats.Contains(imageFormat))
        {
            yield return new ValidationResult(
                $"ImageFile expected to be in the following formats " +
                $"=> '{imageLimitOptions.AllowedUploadFormats.ToDelimitedString(", ")}'");
        }
        
    }
}