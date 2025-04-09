using PetFamily.Core.Dto.Breed;
using PetFamily.Core.Dto.Pet;
using PetFamily.Core.Dto.Species;
using PetFamily.Core.Dto.Volunteer;

namespace PetFamily.Volunteers.Application;

public interface IReadDbContext
{
    IQueryable<VolunteerDto> Volunteers { get; }
    IQueryable<PetDto> Pets { get; }
}