using BasicJira.Api.Contracts.RabbitMq;
using BasicJira.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BasicJira.Contracts.Messages;

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
        var message = new SendEmailMessage
        {
            MessageId = Guid.NewGuid(),
            Recipient = request.Recipient,
            Subject = request.Subject,
            Body = request.Body,
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