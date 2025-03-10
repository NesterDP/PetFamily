using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Application.Dto.Pet;
using PetFamily.Application.Dto.Species;

namespace PetFamily.Infrastructure.Configurations.Read;


public class SpeciesDtoConfiguration : IEntityTypeConfiguration<SpeciesDto>
{
    public void Configure(EntityTypeBuilder<SpeciesDto> builder)
    {
        builder.ToTable("species");
        
        builder.HasKey(p => p.Id);
    }
}