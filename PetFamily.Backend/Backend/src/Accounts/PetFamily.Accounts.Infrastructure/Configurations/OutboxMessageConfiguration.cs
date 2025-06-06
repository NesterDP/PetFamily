using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Accounts.Infrastructure.Outbox;
using PetFamily.SharedKernel.Constants;

namespace PetFamily.Accounts.Infrastructure.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Payload)
            .HasColumnType("jsonb")
            .HasMaxLength(DomainConstants.MAX_HIGH_TEXT_LENGTH)
            .IsRequired();

        builder.Property(o => o.Type)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(o => o.OccurredOnUtc)
            .HasConversion(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
            .IsRequired();

        builder.Property(o => o.ProcessedOnUtc)
            .HasConversion(
                v => v!.Value.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
            .IsRequired(false)
            .HasColumnName("processed_on_utc");

        builder.HasIndex(e => new { e.OccurredOnUtc, e.ProcessedOnUtc })
            .HasDatabaseName("idx_outbox_messages_unprocessed")
            .IncludeProperties(e => new { e.Id, e.Type, e.Payload })
            .HasFilter("processed_on_utc IS NULL");
    }
}