using Microsoft.EntityFrameworkCore;
using PetFamily.Application.Dto.Pet;
using PetFamily.Application.Dto.Volunteer;

namespace PetFamily.Application.Database;

public interface IReadDbContext
{
    DbSet<VolunteerDto> Volunteers { get;}
    DbSet<PetDto> Pets { get;}
}