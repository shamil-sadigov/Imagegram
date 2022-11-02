using Imagegram.Database;
using Imagegram.Database.Entities;
using Imagegram.Features.Users.GetUserAccessToken;
using MediatR;

namespace Imagegram.Features.Users.CreateUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisteredUser>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordManager _passwordManager;

    public RegisterUserCommandHandler(
        ApplicationDbContext dbContext,
        IPasswordManager passwordManager)
    {
        _dbContext = dbContext;
        _passwordManager = passwordManager;
    }
    
    public async Task<RegisteredUser> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var protectedPassword = _passwordManager.ProtectUserPassword(request.Email, request.Password);

        // TODO: Abstract from time

        var newUser = new User()
        {
            Email = request.Email,
            Password = protectedPassword,
            CreatedAt = DateTimeOffset.UtcNow
        };
        
        await _dbContext.Users.AddAsync(newUser, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new RegisteredUser(newUser.Id, newUser.Email);
    }
}