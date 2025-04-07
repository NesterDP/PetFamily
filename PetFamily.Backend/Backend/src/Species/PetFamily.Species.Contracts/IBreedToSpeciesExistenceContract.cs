using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.Species.Contracts.Requests;

namespace PetFamily.Species.Contracts;

public interface IBreedToSpeciesExistenceContract
{
    Task<UnitResult<Error>> BreedToSpeciesExistence(
        BreedToSpeciesExistenceRequest request,
        CancellationToken cancellationToken = default);
}