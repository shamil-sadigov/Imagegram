using Imagegram.Database.Models;
using Microsoft.AspNetCore.DataProtection;

namespace Imagegram.Features.Users;

// TODO: Register in IoC

public class PasswordManager : IPasswordManager
{
    private readonly IDataProtectionProvider _dataProtectionProvider;

    public PasswordManager(IDataProtectionProvider dataProtectionProvider)
    {
        _dataProtectionProvider = dataProtectionProvider;
    }
    
    public bool IsUserPasswordValid(User user, string rawPassword)
    {
        var userRawPassword = _dataProtectionProvider
            .CreateProtector(user.Email)
            .Unprotect(user.Password);
        
        return userRawPassword == rawPassword;
    }
    
    /// <returns>protected rawPassword</returns>
    public string ProtectUserPassword(string userEmail, string rawPassword)
    {
        // Just a lightweight protection
        return _dataProtectionProvider
            .CreateProtector(userEmail)
            .Protect(rawPassword);
    }
}