using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Core.Dto.VolunteerRequest;

namespace PetFamily.VolunteerRequests.Infrastructure.Configurations.Read;

public class VolunteerRequestDtoConfiguration : IEntityTypeConfiguration<VolunteerRequestDto>
{
    public void Configure(EntityTypeBuilder<VolunteerRequestDto> builder)
    {
        builder.ToTable("volunteer_requests");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id)
            .IsRequired()
            .HasColumnName("id");
        
        builder.Property(v => v.AdminId)
            .IsRequired(false)
            .HasColumnName("admin_id");

        builder.Property(v => v.UserId)
            .IsRequired()
            .HasColumnName("user_id");

        builder.Property(v => v.VolunteerInfo)
            .IsRequired()
            .HasColumnName("volunteer_info");
        
        builder.Property(s => s.Status)
            .IsRequired()
            .HasColumnName("status");

        builder.Property(v => v.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");
        
        builder.Property(v => v.RejectedAt)
            .IsRequired(false)
            .HasColumnName("rejected_at");
        
        builder.Property(v => v.RevisionComment)
            .IsRequired(false)
            .HasColumnName("revision_comment");
    }
}