using System.ComponentModel.DataAnnotations;

namespace Imagegram.Requests;

public sealed record RegisterUserRequest
(
    [EmailAddress] string Email, 
    [MinLength(5)] string Password
);