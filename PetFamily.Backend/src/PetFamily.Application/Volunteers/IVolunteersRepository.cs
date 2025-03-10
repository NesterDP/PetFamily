using CSharpFunctionalExtensions;
using PetFamily.Domain.PetContext.Entities;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared.CustomErrors;

namespace PetFamily.Application.Volunteers;

public interface IVolunteersRepository
{
    Task<Guid> AddAsync(Volunteer volunteer, CancellationToken cancellationToken = default);

    Guid Save(Volunteer volunteer, CancellationToken cancellationToken = default);
    
    Guid Delete(Volunteer volunteer, CancellationToken cancellationToken = default);
    
    Task<Result<Volunteer, Error>> GetByIdAsync(VolunteerId id, CancellationToken cancellationToken = default);
}