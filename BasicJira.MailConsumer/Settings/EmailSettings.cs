using System;
using System.Collections.Generic;
using System.Text;

namespace BasicJira.MailConsumer.Settings;

public sealed class EmailSettings
{
    public string Host { get; init; } = string.Empty;

    public int Port { get; init; }

    public string SenderName { get; init; } = string.Empty;

    public string SenderEmail { get; init; } = string.Empty;

    public string UserName { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;
}