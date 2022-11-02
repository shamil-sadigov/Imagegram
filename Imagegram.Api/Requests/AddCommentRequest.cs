using System.ComponentModel.DataAnnotations;

namespace Imagegram.Api.Requests;

public sealed record AddCommentRequest
(
     [Required] 
     [MinLength(1)]
     string CommentText
);