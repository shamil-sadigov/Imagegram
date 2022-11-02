using System.ComponentModel.DataAnnotations;

namespace Imagegram.Api.Requests;

public sealed record GetAccessTokenRequest
(
    [EmailAddress] string Email,
    [Required] string Password
);

public sealed record AccessTokenResponse
(
    [Required] string Token
);