namespace Imagegram.Features.Users.CreateUserAccessToken;

public class InvalidPasswordException:Exception
{
    public string Email { get; }

    public InvalidPasswordException(string email):base(BuildMessage(email))
    {
        Email = email;
    }

    private static string BuildMessage(string email)
    {
        return $"ProtectedPassword provided for email '{email}' is not valid";
    }
}