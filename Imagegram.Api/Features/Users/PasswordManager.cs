using Imagegram.Database.Entities;
using Microsoft.AspNetCore.DataProtection;

namespace Imagegram.Api.Features.Users;


public sealed class PasswordManager : IPasswordManager
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
            .Unprotect(user.ProtectedPassword);
        
        return userRawPassword == rawPassword;
    }
    
    /// <returns>protected <see cref="rawPassword"/></returns>
    public string ProtectUserPassword(string userEmail, string rawPassword)
    {
        // Just a lightweight protection
        return _dataProtectionProvider
            .CreateProtector(userEmail)
            .Protect(rawPassword);
    }
}