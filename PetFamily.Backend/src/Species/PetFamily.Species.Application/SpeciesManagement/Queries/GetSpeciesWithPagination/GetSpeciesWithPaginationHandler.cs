using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Species;
using PetFamily.Core.Extensions;
using PetFamily.Core.Models;

namespace PetFamily.Species.Application.SpeciesManagement.Queries.GetSpeciesWithPagination;

public class GetSpeciesWithPaginationHandler : IQueryHandler<PagedList<SpeciesDto>, GetSpeciesWithPaginationQuery>
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