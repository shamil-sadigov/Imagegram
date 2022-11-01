using Imagegram.Database;
using Imagegram.Database.Models;
using Imagegram.Features.Users.GetUserAccessToken;
using MediatR;

namespace Imagegram.Features.Users.CreateUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisteredUser>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly PasswordManager _passwordManager;

    public RegisterUserCommandHandler(
        ApplicationDbContext dbContext,
        PasswordManager passwordManager)
    {
        _dbContext = dbContext;
        _passwordManager = passwordManager;
    }
    
    public async Task<RegisteredUser> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // Just lightweight protection
        
        var protectedPassword = _passwordManager.ProtectUserPassword(request.Email, request.Password);

        // TODO: Abstract from time

        var newuUser = new User()
        {
            Email = request.Email,
            Password = protectedPassword,
            CreatedAt = DateTimeOffset.UtcNow
        };
        
        await _dbContext.Users.AddAsync(newuUser, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new RegisteredUser(newuUser.Id, newuUser.Email);
    }
}