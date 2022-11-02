using MediatR;

namespace Imagegram.Api.Features.Users.CreateUser;

public sealed record RegisterUserCommand(string Email, string Password) : IRequest<RegisteredUser>;

public sealed record RegisteredUser(int UserId, string Email);