using System.ComponentModel.DataAnnotations;

namespace Imagegram.Requests;

public class RegisterUserRequest
{
    [EmailAddress]
    public string Email { get; set; }
    
    [MinLength(5)]
    public string Password { get; set; }
}