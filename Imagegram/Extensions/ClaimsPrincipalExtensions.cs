using System.Security.Claims;
using Imagegram.Features.Users.GetUserAccessToken.Services;

namespace Imagegram.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.Claims.FirstOrDefault(x=> x.Type == ApplicationClaimTypes.UserId);

        if (userIdClaim is null)
        {
            throw new InvalidOperationException($"'{ApplicationClaimTypes.UserId}' claim type is not available");
        }
        
        if (!int.TryParse(userIdClaim.Value, out var parsedId))
        {
            throw new InvalidOperationException(
                $"'{ApplicationClaimTypes.UserId}' claim type has value of unexpected format. Int expected");
        }
        
        return parsedId;
    } 
}