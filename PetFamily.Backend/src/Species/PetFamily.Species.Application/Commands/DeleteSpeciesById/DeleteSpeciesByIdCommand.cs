using PetFamily.Core.Abstractions;

namespace PetFamily.Species.Application.Commands.DeleteSpeciesById;

public record DeleteSpeciesByIdCommand(Guid Id) : ICommand;