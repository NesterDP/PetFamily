using PetFamily.Core.Abstractions;

namespace PetFamily.Species.Application.Commands.Create;

public record CreateSpeciesCommand(string Name) : ICommand;