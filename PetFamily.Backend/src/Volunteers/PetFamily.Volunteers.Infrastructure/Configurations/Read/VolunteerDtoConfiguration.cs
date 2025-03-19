using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Core.Dto.Pet;
using PetFamily.Core.Dto.Shared;
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
        
        builder.HasMany<PetDto>(v => v.Pets)
            .WithOne()
            .HasForeignKey(p => p.Id)
            .IsRequired(false);
        
        //builder.HasQueryFilter(v => v.)
    }
}