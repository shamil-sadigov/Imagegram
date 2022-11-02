using Imagegram.Database;
using Imagegram.Database.Entities;
using Imagegram.Features.Users.GetUserAccessToken.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Imagegram.Features.Users.GetUserAccessToken;

public class GetUserAccessTokenQueryHandler : IRequestHandler<CreateUserAccessTokenCommand, UserAccessToken>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordManager _passwordManager;
    private readonly IAccessTokenGenerator _accessTokenGenerator;

    public GetUserAccessTokenQueryHandler(
        ApplicationDbContext dbContext,
        IPasswordManager passwordManager,
        IAccessTokenGenerator accessTokenGenerator)
    {
        _dbContext = dbContext;
        _passwordManager = passwordManager;
        _accessTokenGenerator = accessTokenGenerator;
    }
    
    public async Task<UserAccessToken> Handle(CreateUserAccessTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await FindUserAsync(request, cancellationToken);

        if (!_passwordManager.IsUserPasswordValid(user, request.Password))
        {
            throw new InvalidPasswordException(request.Email);
        }

        var generateToken = _accessTokenGenerator.GenerateToken(user);

        return new UserAccessToken(generateToken);
    }

    private async Task<User> FindUserAsync(CreateUserAccessTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);
        
        return user ??  throw new EntityNotFoundException($"User with email '{request.Email}' was not found");
    }
}
