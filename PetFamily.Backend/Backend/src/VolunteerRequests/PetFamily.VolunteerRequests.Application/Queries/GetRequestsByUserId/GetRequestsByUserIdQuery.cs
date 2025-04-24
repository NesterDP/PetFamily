using PetFamily.Core.Abstractions;

namespace PetFamily.VolunteerRequests.Application.Queries.GetRequestsByUserId;

public record GetRequestsByUserIdQuery : IQuery
{
    public Guid UserId { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }

    public string? Status { get; set; }

    public GetRequestsByUserIdQuery(Guid userId, int page, int pageSize, string? status = null)
    {
        UserId = userId;
        Page = page;
        PageSize = pageSize;
        Status = status;
    }
}