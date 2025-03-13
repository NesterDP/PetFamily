using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.Species.Application.Queries.CheckBreedToSpeciesExistence;
using PetFamily.Species.Contracts;
using PetFamily.Species.Contracts.Requests;

namespace PetFamily.Species.Presentation.Contracts;

public class BreedToSpeciesExistenceContract(CheckBreedToSpeciesExistenceHandler handler) : IBreedToSpeciesExistenceContract
{
    public async Task<UnitResult<Error>> BreedToSpeciesExistence(
        BreedToSpeciesExistenceRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = new CheckBreedToSpeciesExistenceQuery(request.SpeciesId, request.BreedId);
        return await handler.HandleAsync(query, cancellationToken);
    }
}