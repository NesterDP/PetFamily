using Microsoft.EntityFrameworkCore;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Database;
using PetFamily.Application.Dto.Breed;

namespace PetFamily.Application.Breeds.Queries.GetBreedsBySpeciesId;

public class GetBreedsBySpeciesIdHandler : IQueryHandler<List<BreedDto>, GetBreedsBySpeciesIdQuery>
{
    private readonly IReadDbContext _readDbContext;

    public GetBreedsBySpeciesIdHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<List<BreedDto>> HandleAsync(
        GetBreedsBySpeciesIdQuery query,
        CancellationToken cancellationToken)
    {
        var result = _readDbContext.Breeds.Where(b => b.SpeciesId == query.Id);
        
        return await result.ToListAsync(cancellationToken);
    }
}