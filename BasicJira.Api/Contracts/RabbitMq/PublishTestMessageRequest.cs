namespace BasicJira.Api.Contracts.RabbitMq;

public sealed record PublishTestMessageRequest(
    string Recipient,
    string Subject,
    string Body);


