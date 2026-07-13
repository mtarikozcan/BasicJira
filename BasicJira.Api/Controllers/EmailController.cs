using BasicJira.Application.Emails.Commands.SendTestEmail;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BasicJira.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
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