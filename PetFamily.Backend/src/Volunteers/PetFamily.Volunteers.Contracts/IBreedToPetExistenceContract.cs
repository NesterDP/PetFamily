using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.Volunteers.Contracts.Requests;

namespace PetFamily.Volunteers.Contracts;

public interface IBreedToPetExistenceContract
{
    Task<UnitResult<Error>> BreedToPetExistence(
        BreedToPetExistenceRequest request,
        CancellationToken cancellationToken = default);
}