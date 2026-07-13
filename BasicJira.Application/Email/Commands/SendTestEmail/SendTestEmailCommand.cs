using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace BasicJira.Application.Emails.Commands.SendTestEmail;

public record SendTestEmailCommand(
    string RecipientEmail,
    string Subject,
    string Body
) : IRequest;