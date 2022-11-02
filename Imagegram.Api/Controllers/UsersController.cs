using Imagegram.Api.Features.Users.CreateUser;
using Imagegram.Api.Features.Users.CreateUserAccessToken;
using Imagegram.Api.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Imagegram.Api.Controllers;

[ApiController]
[Route("api/v1/users")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest request)
    {
        await _mediator.Send(new RegisterUserCommand(request.Email, request.Password));
        
        // TODO: It's better to return 201, but for now it's not important
        return Ok();
    }
    
    [AllowAnonymous]
    [HttpPost("access-token")]
    public async Task<IActionResult> GetAccessToken([FromBody] GetAccessTokenRequest request)
    {
        var userAccessToken = await _mediator.Send(new CreateUserAccessTokenCommand(request.Email, request.Password));

        var accessTokenResponse = new AccessTokenResponse(userAccessToken.Value);

        return Ok(accessTokenResponse);
    }
}