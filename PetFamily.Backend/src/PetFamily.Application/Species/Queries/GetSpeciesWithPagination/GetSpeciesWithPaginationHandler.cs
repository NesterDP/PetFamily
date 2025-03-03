using PetFamily.Application.Database;
using PetFamily.Application.Dto.Species;
using PetFamily.Application.Extensions;
using PetFamily.Application.Models;

namespace PetFamily.Application.Species.Queries.GetSpeciesWithPagination;

public class GetSpeciesWithPaginationHandler
{
    private readonly IReadDbContext _readDbContext;

    public GetSpeciesWithPaginationHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<PagedList<SpeciesDto>> HandleAsync(
        GetSpeciesWithPaginationQuery query,
        CancellationToken cancellationToken)
    {
        var speciesQuery = _readDbContext.Species;

        return await speciesQuery.ToPagedList(query.Page, query.PageSize, cancellationToken);
    }
}