using System;
using System.Collections.Generic;
using System.Text;

using BasicJira.Application.Common.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BasicJira.Infrastructure.Email;

public sealed class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> options)
    {
        // appsettings.json içindeki EmailSettings bölümü
        // strongly typed EmailSettings nesnesine çevrilerek alınır.
        _settings = options.Value;
    }

    public async Task SendAsync(
        string recipientEmail,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        // Gönderilecek e-posta mesajı oluşturulur.
        var message = new MimeMessage();

        // Mailin kimden gönderileceği belirlenir.
        message.From.Add(
            new MailboxAddress(
                _settings.SenderName,
                _settings.SenderEmail));

        // Alıcı adresi eklenir.
        message.To.Add(
            MailboxAddress.Parse(recipientEmail));

        // Mail başlığı belirlenir.
        message.Subject = subject;

        // Mail içeriği HTML formatında hazırlanır.
        message.Body = new BodyBuilder
        {
            HtmlBody = body
        }.ToMessageBody();

        // SMTP bağlantısını yönetecek MailKit client oluşturulur.
        using var smtpClient = new SmtpClient();

        // Port ve sağlayıcı ayarına göre güvenli bağlantı türü seçilir.
        var secureSocketOption = _settings.UseSsl
            ? SecureSocketOptions.SslOnConnect
            : SecureSocketOptions.StartTls;

        // SMTP sunucusuna bağlantı kurulur.
        await smtpClient.ConnectAsync(
            _settings.Host,
            _settings.Port,
            secureSocketOption,
            cancellationToken);

        // SMTP kullanıcı bilgileriyle giriş yapılır.
        await smtpClient.AuthenticateAsync(
            _settings.Username,
            _settings.Password,
            cancellationToken);

        // E-posta gönderilir.
        await smtpClient.SendAsync(
            message,
            cancellationToken);

        // SMTP bağlantısı düzgün şekilde kapatılır.
        await smtpClient.DisconnectAsync(
            true,
            cancellationToken);
    }
}