using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Accounts.Domain.DataModels;

namespace PetFamily.Accounts.Infrastructure.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");

        builder.Property(p => p.Code)
            .HasColumnName("code");

        builder
            .HasIndex(p => p.Code)
            .IsUnique();
    }
}