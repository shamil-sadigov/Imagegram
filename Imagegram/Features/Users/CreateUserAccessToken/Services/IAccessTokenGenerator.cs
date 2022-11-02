using Imagegram.Database.Entities;

namespace Imagegram.Features.Users.CreateUserAccessToken.Services;

public interface IAccessTokenGenerator
{
    string GenerateToken(User user);
}