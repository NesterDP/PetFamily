using PetFamily.Core.Abstractions;

namespace PetFamily.Species.Application.SpeciesManagement.Commands.DeleteSpeciesById;

public record DeleteSpeciesByIdCommand(Guid Id) : ICommand;