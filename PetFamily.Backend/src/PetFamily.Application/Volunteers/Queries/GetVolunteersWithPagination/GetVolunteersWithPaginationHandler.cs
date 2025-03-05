using Microsoft.EntityFrameworkCore;
using PetFamily.Application.Database;
using PetFamily.Application.Dto.Volunteer;
using PetFamily.Application.Extensions;
using PetFamily.Application.Models;

namespace PetFamily.Application.Volunteers.Queries.GetVolunteersWithPagination;

public class GetVolunteersWithPaginationHandler
{
    private readonly IReadDbContext _readDbContext;

    public GetVolunteersWithPaginationHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<PagedList<VolunteerDto>> HandlerAsync(
        GetVolunteersWithPaginationQuery query,
        CancellationToken cancellationToken)
    {
        var volunteersQuery = _readDbContext.Volunteers;

        //var result = await volunteersQuery.ToListAsync();
        
        return await volunteersQuery.ToPagedList(query.Page, query.PageSize, cancellationToken);
    }
}