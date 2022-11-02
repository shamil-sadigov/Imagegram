using System.ComponentModel.DataAnnotations;

namespace Imagegram.Requests;

public sealed class GetAccessTokenRequest
{
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    public string Password { get; set; }
}

public sealed class AccessTokenResponse
{
    [Required]
    public string Token { get; set; }
}