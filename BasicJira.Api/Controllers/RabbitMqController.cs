using BasicJira.Api.Contracts.RabbitMq;
using BasicJira.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BasicJira.Api.Controllers;

[ApiController]
[Route("api/rabbitmq")]
public sealed class RabbitMqController : ControllerBase
{
    private readonly IMessagePublisher _messagePublisher;

    public RabbitMqController(
        IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    [HttpPost("test")]
    public async Task<IActionResult> PublishTestMessage(
        PublishTestMessageRequest request,
        CancellationToken cancellationToken)
    {
        var message = new
        {
            MessageId = Guid.NewGuid(),     //anonymous object , ileride typed message.
            request.Recipient,
            request.Subject,
            request.Body,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _messagePublisher.PublishAsync(
            message,
            cancellationToken);

        return Accepted(new
        {
            message = "Mesaj RabbitMQ'ya publish edildi.",
            data = message
        });
    }
}