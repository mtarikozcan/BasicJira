namespace BasicJira.MailConsumer.Persistence.Entities;

public sealed class ProcessedMessage
{
    public Guid MessageId { get; set; }

    public DateTime ProcessedAtUtc { get; set; }
}