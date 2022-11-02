using Imagegram.Database;
using Imagegram.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Imagegram.Api.Features.Users.CreateUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisteredUser>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordManager _passwordManager;
    private readonly ISystemTime _systemTime;
    public RegisterUserCommandHandler(
        ApplicationDbContext dbContext,
        IPasswordManager passwordManager, 
        ISystemTime systemTime)
    {
        _dbContext = dbContext;
        _passwordManager = passwordManager;
        _systemTime = systemTime;
    }
    
    public async Task<RegisteredUser> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        await EnsureEmailIsUniqueAsync(command, cancellationToken);
        
        var protectedPassword = _passwordManager.ProtectUserPassword(command.Email, command.Password);
        
        var newUser = new User()
        {
            Email = command.Email,
            ProtectedPassword = protectedPassword,
            CreatedAt = _systemTime.CurrentUtc
        };
        
        await _dbContext.Users.AddAsync(newUser, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new RegisteredUser(newUser.Id, newUser.Email);
    }

    private async Task EnsureEmailIsUniqueAsync(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        if (await _dbContext.Users.AnyAsync(x => x.Email == command.Email, cancellationToken))
        {
            throw new DuplicateEmailException(command.Email);
        }
    }
}