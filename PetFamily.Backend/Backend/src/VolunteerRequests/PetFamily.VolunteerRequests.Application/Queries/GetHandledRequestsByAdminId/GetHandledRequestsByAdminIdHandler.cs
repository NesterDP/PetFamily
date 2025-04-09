using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.VolunteerRequest;
using PetFamily.Core.Extensions;
using PetFamily.Core.Models;
using PetFamily.VolunteerRequests.Application.Abstractions;

namespace PetFamily.VolunteerRequests.Application.Queries.GetHandledRequestsByAdminId;

public class GetHandledRequestsByAdminIdHandler
    : IQueryHandler<PagedList<VolunteerRequestDto>, GetHandledRequestsByAdminIdQuery>
{
    private readonly IReadDbContext _readDbContext;

    public GetHandledRequestsByAdminIdHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<PagedList<VolunteerRequestDto>> HandleAsync(
        GetHandledRequestsByAdminIdQuery query,
        CancellationToken cancellationToken)
    {
        var volunteerRequestsQuery = _readDbContext.VolunteerRequests;

        volunteerRequestsQuery = volunteerRequestsQuery.Where(v =>
            v.AdminId == query.AdminId &&
            v.Status == query.Status);

        return await volunteerRequestsQuery.ToPagedList(query.Page, query.PageSize, cancellationToken);
    }
}