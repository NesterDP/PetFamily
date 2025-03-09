using Microsoft.EntityFrameworkCore;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Database;
using PetFamily.Application.Dto.Volunteer;
using PetFamily.Application.Extensions;
using PetFamily.Application.Models;

namespace PetFamily.Application.Volunteers.Queries.GetVolunteersWithPagination;

public class GetVolunteersWithPaginationHandler : IQueryHandler<
    PagedList<VolunteerDto>,
    GetVolunteersWithPaginationQuery>
{
    private readonly IReadDbContext _readDbContext;

    public GetVolunteersWithPaginationHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<PagedList<VolunteerDto>> HandleAsync(
        GetVolunteersWithPaginationQuery query,
        CancellationToken cancellationToken)
    {
        var volunteersQuery = _readDbContext.Volunteers;
        
        volunteersQuery = volunteersQuery.Where(v => v.IsDeleted == false);
        
        return await volunteersQuery.ToPagedList(query.Page, query.PageSize, cancellationToken);
    }
}