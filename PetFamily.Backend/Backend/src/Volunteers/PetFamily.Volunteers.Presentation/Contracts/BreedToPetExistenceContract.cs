using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.Volunteers.Application.Queries.CheckBreedToPetExistence;
using PetFamily.Volunteers.Contracts;
using PetFamily.Volunteers.Contracts.Requests;

namespace PetFamily.Volunteers.Presentation.Contracts;

public class BreedToPetExistenceContract : IBreedToPetExistenceContract
{
    private readonly CheckBreedToPetExistenceHandler _handler;

    public BreedToPetExistenceContract(CheckBreedToPetExistenceHandler handler)
    {
        _handler = handler;
    }

    public async Task<UnitResult<Error>> BreedToPetExistence(
        BreedToPetExistenceRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = new CheckBreedToPetExistenceQuery(request.BreedId);
        return await _handler.HandleAsync(query, cancellationToken);
    }
}