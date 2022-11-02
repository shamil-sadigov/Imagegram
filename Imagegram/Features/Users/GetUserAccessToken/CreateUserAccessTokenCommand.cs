using MediatR;

namespace Imagegram.Features.Users.GetUserAccessToken;

public sealed record CreateUserAccessTokenCommand(string Email, string Password) : IRequest<UserAccessToken>;

public sealed record UserAccessToken(string Value);