using System.ComponentModel.DataAnnotations;

namespace Imagegram.Requests;

public sealed record AddCommentRequest
(
     [Required] 
     [MinLength(1)]
     string CommentText
);