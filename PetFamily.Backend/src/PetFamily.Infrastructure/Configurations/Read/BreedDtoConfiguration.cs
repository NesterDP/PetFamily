using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Application.Dto.Breed;

namespace PetFamily.Infrastructure.Configurations.Read;

public class BreedDtoConfiguration : IEntityTypeConfiguration<BreedDto>
{
    public void Configure(EntityTypeBuilder<BreedDto> builder)
    {
        builder.ToTable("breeds");
        
        builder.HasKey(p => p.Id);
    }
}