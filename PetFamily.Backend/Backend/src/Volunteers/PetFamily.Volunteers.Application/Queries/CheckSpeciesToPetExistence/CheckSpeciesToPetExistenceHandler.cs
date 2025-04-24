using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using PetFamily.Core.Abstractions;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Volunteers.Application.Queries.CheckSpeciesToPetExistence;

public class CheckSpeciesToPetExistenceHandler : IQueryHandler<UnitResult<Error>, CheckSpeciesToPetExistenceQuery>
{
    private readonly IReadDbContext _readDbContext;

    public CheckSpeciesToPetExistenceHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<UnitResult<Error>> HandleAsync(
        CheckSpeciesToPetExistenceQuery query,
        CancellationToken cancellationToken)
    {
        var isUsed = await _readDbContext.Pets.FirstOrDefaultAsync(
            p => p.SpeciesId == query.SpeciesId, cancellationToken);

        if (isUsed != null)
            return Errors.General.Conflict($"pets with SpeciesId = {query.SpeciesId} are still in database");

        return UnitResult.Success<Error>();
    }
}