using PetFamily.Core.Abstractions;
using PetFamily.VolunteerRequests.Domain.ValueObjects;

namespace PetFamily.VolunteerRequests.Application.Queries.GetHandledRequestsByAdminId;

public record GetHandledRequestsByAdminIdQuery : IQuery
{
    public Guid AdminId { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }

    public string? Status { get; set; }

    public GetHandledRequestsByAdminIdQuery(Guid adminId, int page, int pageSize, string? status = null)
    {
        AdminId = adminId;
        Page = page;
        PageSize = pageSize;
        Status = status;
        if (status is null)
            Status = VolunteerRequestStatusEnum.OnReview.ToString();
    }
}