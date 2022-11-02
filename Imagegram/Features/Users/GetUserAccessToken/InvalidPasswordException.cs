namespace Imagegram.Features.Users.GetUserAccessToken;

public class InvalidPasswordException:Exception
{
    public string Email { get; }

    public InvalidPasswordException(string email):base(BuildMessage(email))
    {
        Email = email;
    }

    private static string BuildMessage(string email)
    {
        return $"Password provided for email '{email}' is not valid";
    }
}