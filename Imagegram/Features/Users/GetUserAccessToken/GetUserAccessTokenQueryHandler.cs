using Imagegram.Database;
using Imagegram.Features.Users.CreateUser;
using MediatR;
using Microsoft.AspNetCore.DataProtection;

namespace Imagegram.Features.Users.GetUserAccessToken;


public class GetUserAccessTokenQueryHandler : IRequestHandler<GetUserAccessTokenQuery, UserAccessToken>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly PasswordManager _passwordManager;
    private readonly AccessTokenGenerator _accessTokenGenerator;

    public GetUserAccessTokenQueryHandler(
        ApplicationDbContext dbContext,
        PasswordManager passwordManager,
        AccessTokenGenerator accessTokenGenerator)
    {
        _dbContext = dbContext;
        _passwordManager = passwordManager;
        _accessTokenGenerator = accessTokenGenerator;
    }
    
    public Task<UserAccessToken> Handle(GetUserAccessTokenQuery request, CancellationToken cancellationToken)
    {
        var user = _dbContext.Users.FirstOrDefault(x=> x.Email == request.Email);

        if (user is null)
        {
            throw new EntityNotFoundException($"User with email '{request.Email}' was not found");
        }
        
        if (!_passwordManager.IsValidPassword(user, request.Password))
        {
            throw new InvalidOperationException($"Provided password for user with email '{request.Email}' is not valid");
        }

        var generateToken = _accessTokenGenerator.GenerateToken(user);

        return Task.FromResult(new UserAccessToken(generateToken));
    }
}
