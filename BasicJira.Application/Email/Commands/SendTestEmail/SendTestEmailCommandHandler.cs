using System;
using System.Collections.Generic;
using System.Text;
using BasicJira.Application.Common.Interfaces;
using MediatR;

namespace BasicJira.Application.Emails.Commands.SendTestEmail;

public sealed class SendTestEmailCommandHandler
    : IRequestHandler<SendTestEmailCommand>
{
    private readonly IEmailService _emailService;

    public SendTestEmailCommandHandler(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Handle(
        SendTestEmailCommand request,
        CancellationToken cancellationToken)
    {
        await _emailService.SendAsync(
            request.RecipientEmail,
            request.Subject,
            request.Body,
            cancellationToken);
    }
}