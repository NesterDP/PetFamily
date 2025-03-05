using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Volunteers.Queries.GetVolunteerById;

public record GetVolunteerByIdQuery(Guid Id) : IQuery;