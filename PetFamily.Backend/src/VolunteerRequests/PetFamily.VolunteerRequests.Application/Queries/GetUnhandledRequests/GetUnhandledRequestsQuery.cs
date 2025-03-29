using PetFamily.Core.Abstractions;

namespace PetFamily.VolunteerRequests.Application.Queries.GetUnhandledRequests;

public record GetUnhandledRequestsQuery(int Page, int PageSize) : IQuery;