using BasicJira.Contracts.Messages;
using BasicJira.MailConsumer.Interfaces;
using BasicJira.MailConsumer.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Polly;
using Polly.Registry;

namespace BasicJira.MailConsumer.Services;

public sealed class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;
    private readonly ResiliencePipeline _retryPipeline;

    public EmailService(
        IOptions<EmailSettings> options,
        ILogger<EmailService> logger,
        ResiliencePipelineProvider<string> pipelineProvider)
    {
        _settings = options.Value;
        _logger = logger;
        _retryPipeline = pipelineProvider.GetPipeline("email-retry");
    }

    public async Task SendAsync(
        SendEmailMessage message,
        CancellationToken cancellationToken)
    {
        await _retryPipeline.ExecuteAsync(
            async token =>
            {
                await SendEmailCoreAsync(message, token);
            },
            cancellationToken);
    }

    private async Task SendEmailCoreAsync(
        SendEmailMessage message,
        CancellationToken cancellationToken)
    {
        var email = new MimeMessage();

        email.From.Add(
            new MailboxAddress(
                _settings.SenderName,
                _settings.SenderEmail));

        email.To.Add(
            MailboxAddress.Parse(message.Recipient));

        email.Subject = message.Subject;

        email.Body = new TextPart("plain")
        {
            Text = message.Body
        };

        using var smtpClient = new SmtpClient();

        await smtpClient.ConnectAsync(
            _settings.Host,
            _settings.Port,
            SecureSocketOptions.StartTls,
            cancellationToken);

        await smtpClient.AuthenticateAsync(
            _settings.UserName,
            _settings.Password,
            cancellationToken);

        await smtpClient.SendAsync(
            email,
            cancellationToken);

        await smtpClient.DisconnectAsync(
            quit: true,
            cancellationToken);

        _logger.LogInformation(
            "E-posta başarıyla gönderildi. MessageId: {MessageId}, Recipient: {Recipient}",
            message.MessageId,
            message.Recipient);
    }
}