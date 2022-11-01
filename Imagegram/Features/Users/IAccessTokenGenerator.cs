using Imagegram.Database.Models;

namespace Imagegram.Features.Users;

public interface IAccessTokenGenerator
{
    string GenerateToken(User user);
}