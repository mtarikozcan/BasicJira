using BasicJira.Application.Common.Authorization;
using BasicJira.Application.Features.Auth.Commands.Login;
using BasicJira.Application.Features.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    [Authorize(Roles = Roles.Admin)]
    [HttpGet("admin")]
    public IActionResult AdminOnly()
    {
        return Ok(new
        {
            Message = "Admin endpoint erişimi başarılı."
        });
    }

    [Authorize(Roles = Roles.User)]
    [HttpGet("user")]
    public IActionResult UserOnly()
    {
        return Ok(new
        {
            Message = "User endpoint erişimi başarılı."
        });
    }

    [Authorize(Roles = Roles.Admin + "," + Roles.User)]
    [HttpGet("profile")]
    public IActionResult Profile()
    {
        return Ok(new
        {
            Message = "Giriş yapmış kullanıcı erişimi başarılı."
        });
    }
}