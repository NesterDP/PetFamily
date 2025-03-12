using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Species.Commands.DeleteSpeciesById;

public record DeleteSpeciesByIdCommand(Guid Id) : ICommand;