using BasicJira.Application.Features.Auth.Commands.Login;
using BasicJira.Application.Features.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BasicJira.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterCommand command,
        CancellationToken cancellationToken)
    {
        var userId = await _sender.Send(command, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, new
        {
            userId
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        var token = await _sender.Send(command, cancellationToken);

        return Ok(new
        {
            token
        });
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            isAuthenticated = User.Identity?.IsAuthenticated,
            claims = User.Claims.Select(claim => new
            {
                claim.Type,
                claim.Value
            })
        });
    }
}