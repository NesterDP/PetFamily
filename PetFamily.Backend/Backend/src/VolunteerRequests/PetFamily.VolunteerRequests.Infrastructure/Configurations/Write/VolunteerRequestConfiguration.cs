using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.SharedKernel.ValueObjects.Ids;
using PetFamily.VolunteerRequests.Domain.Entities;
using PetFamily.VolunteerRequests.Domain.ValueObjects;

namespace PetFamily.VolunteerRequests.Infrastructure.Configurations.Write;

public class VolunteerRequestConfiguration : IEntityTypeConfiguration<VolunteerRequest>
{
    public void Configure(EntityTypeBuilder<VolunteerRequest> builder)
    {
        builder.ToTable("volunteer_requests");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id)
            .HasConversion(
                id => id.Value,
                value => VolunteerRequestId.Create(value))
            .IsRequired()
            .HasColumnName("id");
        
        builder.Property(v => v.AdminId)
            .HasConversion(
                id => id.Value,
                value => AdminId.Create(value))
            .IsRequired(false)
            .HasColumnName("admin_id");

        builder.Property(v => v.UserId)
            .HasConversion(
                id => id.Value,
                value => UserId.Create(value))
            .IsRequired()
            .HasColumnName("user_id");

        builder.ComplexProperty(v => v.VolunteerInfo, vb =>
        {
            vb.Property(v => v.Value)
                .IsRequired()
                .HasColumnName("volunteer_info");
        });
        
        builder.ComplexProperty(v => v.Status, sb =>
        {
            sb.Property(s => s.Value)
                .IsRequired()
                .HasConversion<string>()
                .HasColumnName("status");
        });

        builder.Property(v => v.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");
        
        builder.Property(v => v.RejectedAt)
            .IsRequired(false)
            .HasColumnName("rejected_at");
        
        builder.Property(v => v.RevisionComment)
            .HasConversion(
                revisionComment => revisionComment.Value,
                value => RevisionComment.Create(value).Value)
            .IsRequired(false)
            .HasColumnName("revision_comment");
    }
}