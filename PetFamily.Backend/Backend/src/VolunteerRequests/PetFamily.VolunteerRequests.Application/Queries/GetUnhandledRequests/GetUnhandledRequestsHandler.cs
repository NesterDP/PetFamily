using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.VolunteerRequest;
using PetFamily.Core.Extensions;
using PetFamily.Core.Models;
using PetFamily.VolunteerRequests.Application.Abstractions;

namespace PetFamily.VolunteerRequests.Application.Queries.GetUnhandledRequests;

public class GetUnhandledRequestsHandler
    : IQueryHandler<PagedList<VolunteerRequestDto>, GetUnhandledRequestsQuery>
{
    private readonly IReadDbContext _readDbContext;

    public GetUnhandledRequestsHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<PagedList<VolunteerRequestDto>> HandleAsync(
        GetUnhandledRequestsQuery query,
        CancellationToken cancellationToken)
    {
        var volunteerRequestsQuery = _readDbContext.VolunteerRequests;

        volunteerRequestsQuery = volunteerRequestsQuery.Where(v => v.AdminId == null);

        return await volunteerRequestsQuery.ToPagedList(query.Page, query.PageSize, cancellationToken);
    }
}