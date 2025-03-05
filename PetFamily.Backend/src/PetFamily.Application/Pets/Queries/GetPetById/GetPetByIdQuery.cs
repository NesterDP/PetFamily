using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Pets.Queries.GetPetById;

public record GetPetByIdQuery(Guid Id) : IQuery;