using System.ComponentModel.DataAnnotations;

namespace Imagegram.Api.Requests;

public sealed record RegisterUserRequest
(
    [EmailAddress] string Email, 
    [MinLength(5)] string Password
);