using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.SharedKernel.ValueObjects.Ids;
using PetFamily.VolunteerRequests.Domain.Entities;

namespace PetFamily.VolunteerRequests.Infrastructure.Configurations.Write;

public class TestEntityConfiguration : IEntityTypeConfiguration<TestEntity>
{
    public void Configure(EntityTypeBuilder<TestEntity> builder)
    {
        builder.ToTable("test_entities");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id)
            .HasConversion(
                id => id.Value,
                value => TestId.Create(value))
            .IsRequired()
            .HasColumnName("id");

        builder.Property(t => t.Status)
            .IsRequired()
            .HasColumnName("status");

        builder.Property(v => v.UserId)
            .HasConversion(
                id => id.Value,
                value => UserId.Create(value))
            .IsRequired()
            .HasColumnName("user_id");

        builder.Property(v => v.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");
    }
}