using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Core.Dto.Volunteer;

namespace PetFamily.Volunteers.Infrastructure.Configurations.Read;

public class VolunteerDtoConfiguration : IEntityTypeConfiguration<VolunteerDto>
{
    public void Configure(EntityTypeBuilder<VolunteerDto> builder)
    {
        builder.ToTable("volunteers");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.IsDeleted)
            .HasColumnName("is_deleted");

        /*builder.HasMany(v => v.Pets)
            .WithOne()
            .HasForeignKey(p => p.OwnerId)
            .IsRequired();*/
    }
}