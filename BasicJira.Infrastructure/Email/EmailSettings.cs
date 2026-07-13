using System;
using System.Collections.Generic;
using System.Text;

namespace BasicJira.Infrastructure.Email;

public sealed class EmailSettings
{
    // appsettings.json içindeki bölüm adıyla eşleşir.
    public const string SectionName = "EmailSettings";

    public string Host { get; set; } = string.Empty;

    public int Port { get; set; }

    public string SenderName { get; set; } = string.Empty;

    public string SenderEmail { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public bool UseSsl { get; set; }
}