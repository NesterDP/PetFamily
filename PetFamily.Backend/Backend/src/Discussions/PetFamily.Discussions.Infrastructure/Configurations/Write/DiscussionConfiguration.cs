using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Core.Extensions.EfCoreFluentApiExtensions;
using PetFamily.Discussions.Domain.Entities;
using PetFamily.Discussions.Domain.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace PetFamily.Discussions.Infrastructure.Configurations.Write;

public class DiscussionConfiguration : IEntityTypeConfiguration<Discussion>
{
    public void Configure(EntityTypeBuilder<Discussion> builder)
    {
        builder.ToTable("discussions");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .HasConversion(
                id => id.Value,
                value => DiscussionId.Create(value))
            .IsRequired()
            .HasColumnName("id");

        builder.Property(d => d.RelationId)
            .HasConversion(
                id => id.Value,
                value => RelationId.Create(value))
            .IsRequired()
            .HasColumnName("relation_id");

        builder.ComplexProperty(d => d.Status, sb =>
        {
            sb.Property(s => s.Value)
                .IsRequired()
                .HasConversion<string>()
                .HasColumnName("status");
        });

        builder.HasMany(d => d.Messages)
            .WithOne()
            .HasForeignKey("discussion_id")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(d => d.Messages).AutoInclude();

        /*builder.OwnsMany(
            d => d.UserIds,
            userIdBuilder =>
            {
                userIdBuilder.ToTable("discussion_users");
                userIdBuilder.WithOwner().HasForeignKey("discussion_id");
                userIdBuilder.Property(u => u.Value).HasColumnName("user_id");
            });*/

        builder.Property(d => d.UserIds)
            .CustomListJsonCollectionConverter(
                value => value.Value,
                dto => UserId.Create(dto))
            .IsRequired()
            .HasColumnName("user_ids")
            .HasColumnType("jsonb");
    }
}