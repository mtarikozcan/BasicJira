using BasicJira.Contracts.Messages;

namespace BasicJira.MailConsumer.Interfaces;

public interface IEmailService
{
    Task SendAsync(
        SendEmailMessage message,
        CancellationToken cancellationToken);
}