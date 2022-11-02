using System.ComponentModel.DataAnnotations;

namespace Imagegram.Requests;

public sealed record GetAccessTokenRequest
(
    [EmailAddress] string Email,
    [Required] string Password
);

public sealed record AccessTokenResponse
(
    [Required] string Token
);