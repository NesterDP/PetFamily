using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Core.Dto.Discussion;

namespace PetFamily.Discussions.Infrastructure.Configurations.Read;

public class DiscussionDtoConfiguration : IEntityTypeConfiguration<DiscussionDto>
{
    public void Configure(EntityTypeBuilder<DiscussionDto> builder)
    {
        builder.ToTable("discussions");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .IsRequired()
            .HasColumnName("id");

        builder.Property(d => d.RelationId)
            .IsRequired()
            .HasColumnName("relation_id");

        builder.Property(d => d.UserIds)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<List<Guid>>(v, JsonSerializerOptions.Default)!)
            .IsRequired()
            .HasColumnName("user_ids")
            .HasColumnType("jsonb");
        
        builder.HasMany(d => d.Messages)
            .WithOne()
            .HasForeignKey("discussion_id")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(d => d.Messages).AutoInclude();
        
        builder.Property(d => d.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasColumnName("status");
    }
}