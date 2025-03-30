using PetFamily.Core.Dto.VolunteerRequest;

namespace PetFamily.VolunteerRequests.Application.Abstractions;

public interface IReadDbContext
{
    IQueryable<VolunteerRequestDto> VolunteerRequests { get; }
}