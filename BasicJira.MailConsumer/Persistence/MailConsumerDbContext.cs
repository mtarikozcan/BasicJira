using BasicJira.MailConsumer.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace BasicJira.MailConsumer.Persistence;

public sealed class MailConsumerDbContext : DbContext
{
    public MailConsumerDbContext(
        DbContextOptions<MailConsumerDbContext> options)
        : base(options)
    {
    }

    public DbSet<ProcessedMessage> ProcessedMessages =>
        Set<ProcessedMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProcessedMessage>(entity =>
        {
            entity.HasKey(x => x.MessageId);

            entity.Property(x => x.ProcessedAtUtc)
                .IsRequired();
        });
    }
}