using Imagegram.Api.Features.Users.CreateUserAccessToken.Services;
using Imagegram.Database;
using Imagegram.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Imagegram.Api.Features.Users.CreateUserAccessToken;

public class CreateUserAccessTokenQueryHandler : IRequestHandler<CreateUserAccessTokenCommand, UserAccessToken>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordManager _passwordManager;
    private readonly IAccessTokenGenerator _accessTokenGenerator;

    public CreateUserAccessTokenQueryHandler(
        ApplicationDbContext dbContext,
        IPasswordManager passwordManager,
        IAccessTokenGenerator accessTokenGenerator)
    {
        _dbContext = dbContext;
        _passwordManager = passwordManager;
        _accessTokenGenerator = accessTokenGenerator;
    }
    
    public async Task<UserAccessToken> Handle(CreateUserAccessTokenCommand command, CancellationToken cancellationToken)
    {
        var user = await FindUserAsync(command, cancellationToken);

        if (!_passwordManager.IsUserPasswordValid(user, command.Password))
        {
            throw new InvalidPasswordException(command.Email);
        }

        var generateToken = _accessTokenGenerator.GenerateToken(user);

        return new UserAccessToken(generateToken);
    }

    private async Task<User> FindUserAsync(CreateUserAccessTokenCommand command, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Email == command.Email, cancellationToken);
        
        return user ??  throw new EntityNotFoundException($"User with email '{command.Email}' was not found");
    }
}
