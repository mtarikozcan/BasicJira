using BasicJira.Application.Emails.Commands.SendTestEmail;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using BasicJira.Application.Common.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace BasicJira.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class EmailsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmailsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("test")]
    public async Task<IActionResult> SendTestEmail(
        SendTestEmailCommand command,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);

        return Ok(new
        {
            message = "Email sent successfully."
        });
    }
}