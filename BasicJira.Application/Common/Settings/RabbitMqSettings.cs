using System;
using System.Collections.Generic;
using System.Text;

namespace BasicJira.Application.Common.Settings;

public sealed class RabbitMqSettings
{
    public const string SectionName = "RabbitMq";

    public string HostName { get; init; } = "localhost";

    public int Port { get; init; } = 5672;

    public string UserName { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;

    public string ExchangeName { get; init; } = "basicjira.mail.exchange";

    public string QueueName { get; init; } = "basicjira.mail.queue";

    public string RoutingKey { get; init; } = "mail.send";
}