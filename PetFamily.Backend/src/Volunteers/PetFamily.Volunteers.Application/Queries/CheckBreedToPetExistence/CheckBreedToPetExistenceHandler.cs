using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using PetFamily.Core.Abstractions;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Volunteers.Application.Queries.CheckBreedToPetExistence;


public class CheckBreedToPetExistenceHandler : IQueryHandler<UnitResult<Error>, CheckBreedToPetExistenceQuery>
{
    private readonly IReadDbContext _readDbContext;

    public CheckBreedToPetExistenceHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<UnitResult<Error>> HandleAsync(
        CheckBreedToPetExistenceQuery query,
        CancellationToken cancellationToken)
    {
        var isUsed = await _readDbContext.Pets.
            FirstOrDefaultAsync(p => p.BreedId == query.BreedId, cancellationToken);
        
        if (isUsed != null)
            return Errors.General.Conflict($"pets with BreedId = {query.BreedId} are still in database");

        return UnitResult.Success<Error>();
    }
}