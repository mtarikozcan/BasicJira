using System;
using System.Collections.Generic;
using System.Text;

namespace BasicJira.Contracts.Messages;
public sealed record SendEmailMessage
{
    public Guid MessageId { get; init; }

    public string Recipient { get; init; } = string.Empty;

    public string Subject { get; init; } = string.Empty;

    public string Body { get; init; } = string.Empty;

    public DateTime CreatedAtUtc { get; init; }
}