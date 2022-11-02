using Imagegram.Database;
using Imagegram.Features.Users.GetUserAccessToken.Services;
using MediatR;

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
    
    public Task<UserAccessToken> Handle(CreateUserAccessTokenCommand request, CancellationToken cancellationToken)
    {
        var user = _dbContext.Users.FirstOrDefault(x=> x.Email == request.Email);

        if (user is null)
        {
            throw new EntityNotFoundException($"User with email '{request.Email}' was not found");
        }
        
        if (!_passwordManager.IsUserPasswordValid(user, request.Password))
        {
            throw new InvalidOperationException($"Provided password for user with email '{request.Email}' is not valid");
        }

        var generateToken = _accessTokenGenerator.GenerateToken(user);

        return Task.FromResult(new UserAccessToken(generateToken));
    }
}
