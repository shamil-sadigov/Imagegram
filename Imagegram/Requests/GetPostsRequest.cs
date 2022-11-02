using System.ComponentModel.DataAnnotations;
using Imagegram.Features;
using Imagegram.Features.Posts.GetPaginated.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace Imagegram.Requests;

/// <summary>
/// Only on of the cursors should be set or non of them.
/// If none of the cursor are set, then it's considered as first page.
/// If client want to navigate to the next page, it should provide 'Limit' and 'AfterCursor'
/// If client want to navigate to the previous page, it should provide 'Limit' and 'BeforeCursor'
///
/// For more details see here => TODO: Add link
/// </summary>
public record GetPostsRequest:IValidatableObject
{
    /// <summary>
    /// How many elements should be shown per page
    /// </summary>
    [Range(PageSize.MinAllowedPageSize, PageSize.MaxAllowedPageSize)]
    [FromQuery(Name = "limit")]
    public int Limit { get; set; }
    
    [FromQuery(Name = "before")]
    public string? BeforeCursor { get; set; } 
    
    [FromQuery(Name = "after")]
    public string? AfterCursor { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrWhiteSpace(BeforeCursor) && !string.IsNullOrWhiteSpace(AfterCursor))
        {
            yield return new ValidationResult("Specify only one cursor. Either 'BeforeCursor' or 'AfterCursor'.");
            yield break;
        }
        
        if (!string.IsNullOrWhiteSpace(BeforeCursor) && 
            !PostCursor.TryCreateFromUrlEncoded(BeforeCursor, out _))
        {
            yield return new ValidationResult("'BeforeCursor' cursor has invalid format");
        }
        
        if (!string.IsNullOrWhiteSpace(AfterCursor) && 
            !PostCursor.TryCreateFromUrlEncoded(AfterCursor, out _))
        {
            yield return new ValidationResult("'AfterCursor' cursor has invalid format");
        }
    }
}

public sealed record PaginatedPostsResponse
(
    int RequestedPageSize,
    int ActualPageSize,
    bool HasMoreItems,
    IEnumerable<PostDto> Items,
    CursorsResponse Cursors
);

public sealed record CursorsResponse(string? StartCursor, string? EndCursor);
