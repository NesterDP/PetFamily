using PetFamily.Core.Dto.Breed;
using PetFamily.Core.Dto.Species;

namespace PetFamily.Species.Application;

public interface IReadDbContext
{
    IQueryable<SpeciesDto> Species { get; }
    IQueryable<BreedDto> Breeds { get; }
}