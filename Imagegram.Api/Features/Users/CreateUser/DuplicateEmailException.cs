namespace Imagegram.Api.Features.Users.CreateUser;

public class DuplicateEmailException:Exception
{
    public string Email { get; }

    public DuplicateEmailException(string email):base(BuildMessage(email))
    {
        Email = email;
    }

    private static string BuildMessage(string email)
        => $"User with email '{email}' is already registered";
}