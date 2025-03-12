using PetFamily.Core.Abstractions;

namespace PetFamily.Volunteers.Application.VolunteersManagement.Queries.GetVolunteerById;

public record GetVolunteerByIdQuery(Guid Id) : IQuery;