using Imagegram.Database.Models;
using Microsoft.AspNetCore.DataProtection;

namespace Imagegram.Features.Users.GetUserAccessToken;

// TODO: Register in IoC

public class PasswordManager
{
    private readonly IDataProtector _dataProtector;

    public PasswordManager(IDataProtector dataProtector)
    {
        _dataProtector = dataProtector;
    }
    
    public bool IsValidPassword(User user, string password)
    {
        var userPasswordProtector = _dataProtector.CreateProtector(user.Email);

        return user.Password == userPasswordProtector.Protect(password);
    }
    
    /// <returns>protected password</returns>
    public string ProtectUserPassword(string userEmail, string rawPassword)
    {
        // Just a lightweight protection
        return _dataProtector
            .CreateProtector(userEmail)
            .Protect(rawPassword);
    }
}