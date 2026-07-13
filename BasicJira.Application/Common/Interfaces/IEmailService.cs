using System;
using System.Collections.Generic;
using System.Text;

namespace BasicJira.Application.Common.Interfaces;

public interface IEmailService
{
    // Application katmanı SMTP, MailKit veya başka bir provider bilmez.
    // Sadece "mail gönderme" sözleşmesini bilir.
    Task SendAsync(
        string recipientEmail,
        string subject,
        string body,
        CancellationToken cancellationToken = default);
}
