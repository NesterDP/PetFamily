using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Discussions.Domain.Entities;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace PetFamily.Discussions.Infrastructure.Configurations.Write;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("messages");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .HasConversion(
                id => id.Value,
                value => MessageId.Create(value))
            .IsRequired()
            .HasColumnName("id");

        builder.ComplexProperty(
            m => m.Text,
            tb =>
            {
                tb.Property(t => t.Value)
                    .IsRequired()
                    .HasMaxLength(DomainConstants.MAX_MEDIUM_TEXT_LENGTH)
                    .HasColumnName("text");
            });

        builder.Property(m => m.UserId)
            .HasConversion(
                id => id.Value,
                value => UserId.Create(value))
            .IsRequired()
            .HasColumnName("user_id");

        builder.Property(m => m.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.ComplexProperty(
            m => m.IsEdited,
            ieb =>
            {
                ieb.Property(ie => ie.Value)
                    .IsRequired()
                    .HasColumnName("is_edited");
            });
    }
}