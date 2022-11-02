using System.Diagnostics;
using Imagegram.Features.Posts.CreatePost;
using Microsoft.AspNetCore.Mvc;

namespace Imagegram.Controllers;

[ApiController]
[Route("[controller]")]
public class PostController : ControllerBase
{
    private readonly ILogger<PostController> _logger;
    
    [HttpPost]
    [RequestFormLimits(MultipartBodyLengthLimit = 268435456)]
    [RequestSizeLimit(268435456)]
    public async Task<IActionResult> Get([FromForm] CreatePostCommand command)
    {
        var startNew = Stopwatch.StartNew();

        
        return Ok("Pong");
    }
}