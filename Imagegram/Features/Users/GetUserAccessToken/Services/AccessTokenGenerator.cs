using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Imagegram.Database.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Imagegram.Features.Users.GetUserAccessToken.Services;

public sealed class AccessTokenGenerator : IAccessTokenGenerator
{
    private readonly AccessTokenOptions _accessTokenOptions;

    public AccessTokenGenerator(IOptions<AccessTokenOptions> accessTokenOptions)
    {
        _accessTokenOptions = accessTokenOptions.Value;
    }
        
    public string GenerateToken(User user)
    {
        if (user is null)
            throw new ArgumentNullException(nameof(user));

        Claim[] basicUserClaims =
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new("UserId", user.Id.ToString()),
            new("Email", user.Email)
        };
            
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(basicUserClaims),
            Audience = _accessTokenOptions.AppName,
            Issuer = _accessTokenOptions.AppName,
            // Expires = DateTime.UtcNow + _accessTokenOptions.TokenLifetime, // NO need
            SigningCredentials = 
                new SigningCredentials(
                    key: new SymmetricSecurityKey(_accessTokenOptions.GetSecretBytes()),
                    algorithm: SecurityAlgorithms.HmacSha256)
        };

        var jwtTokenHandler = new JwtSecurityTokenHandler();

        SecurityToken securityToken = jwtTokenHandler.CreateToken(tokenDescriptor);

        return jwtTokenHandler.WriteToken(securityToken);
    }
}