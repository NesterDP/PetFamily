using Microsoft.EntityFrameworkCore;
using PetFamily.Application.Dto.Breed;
using PetFamily.Application.Dto.Pet;
using PetFamily.Application.Dto.Species;
using PetFamily.Application.Dto.Volunteer;

namespace PetFamily.Application.Database;

public interface IReadDbContext
{
    IQueryable<VolunteerDto> Volunteers { get; }
    IQueryable<PetDto> Pets { get; }
    IQueryable<SpeciesDto> Species { get; }
    IQueryable<BreedDto> Breeds { get; }
}