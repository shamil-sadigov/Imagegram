using Imagegram.Database.Entities;

namespace Imagegram.Api.Features.Users.CreateUserAccessToken.Services;

public interface IAccessTokenGenerator
{
    string GenerateToken(User user);
}