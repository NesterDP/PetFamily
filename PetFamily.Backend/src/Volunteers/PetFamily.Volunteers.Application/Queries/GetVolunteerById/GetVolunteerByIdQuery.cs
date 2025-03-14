using PetFamily.Core.Abstractions;

namespace PetFamily.Volunteers.Application.Queries.GetVolunteerById;

public record GetVolunteerByIdQuery(Guid Id) : IQuery;