using Imagegram.Database.Entities;

namespace Imagegram.Features.Users;

public interface IPasswordManager
{
    bool IsUserPasswordValid(User user, string rawPassword);

    /// <returns>protected rawPassword</returns>
    string ProtectUserPassword(string userEmail, string rawPassword);
}