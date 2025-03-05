using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Pets.Queries;

public record GetPetByIdQuery(Guid Id) : IQuery;