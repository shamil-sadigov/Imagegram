using Imagegram.Database.Entities;

namespace Imagegram.Features.Users.GetUserAccessToken.Services;

public interface IAccessTokenGenerator
{
    string GenerateToken(User user);
}