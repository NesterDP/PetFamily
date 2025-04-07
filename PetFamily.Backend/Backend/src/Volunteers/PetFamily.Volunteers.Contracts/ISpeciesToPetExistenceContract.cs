using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.Volunteers.Contracts.Requests;

namespace PetFamily.Volunteers.Contracts;

public interface ISpeciesToPetExistenceContract
{
    Task<UnitResult<Error>> SpeciesToPetExistence(
        SpeciesToPetExistenceRequest request,
        CancellationToken cancellationToken = default);
}