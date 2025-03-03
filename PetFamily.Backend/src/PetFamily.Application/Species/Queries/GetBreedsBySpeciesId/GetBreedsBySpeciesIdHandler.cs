using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using PetFamily.Application.Database;
using PetFamily.Application.Dto.Breed;
using PetFamily.Domain.Shared.CustomErrors;

namespace PetFamily.Application.Species.Queries.GetBreedsBySpeciesId;

public class GetBreedsBySpeciesIdHandler
{
    private readonly IReadDbContext _readDbContext;

    public GetBreedsBySpeciesIdHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<List<BreedDto>> HandlerAsync(
        GetBreedsBySpeciesIdQuery query,
        CancellationToken cancellationToken)
    {
        var result = _readDbContext.Breeds.Where(b => b.SpeciesId == query.Id);
        
        return await result.ToListAsync(cancellationToken);
    }
}