using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.ValueObjects.Ids;
using PetFamily.Volunteers.Domain.Entities;
using PetFamily.Volunteers.Domain.ValueObjects.VolunteerVO;

namespace PetFamily.Volunteers.Application;

public interface IVolunteersRepository
{
    Task<Guid> AddAsync(Volunteer volunteer, CancellationToken cancellationToken = default);

    Guid Save(Volunteer volunteer, CancellationToken cancellationToken = default);
    
    Guid Delete(Volunteer volunteer, CancellationToken cancellationToken = default);
    
    Task<Result<Volunteer, Error>> GetByIdAsync(VolunteerId id, CancellationToken cancellationToken = default);
}