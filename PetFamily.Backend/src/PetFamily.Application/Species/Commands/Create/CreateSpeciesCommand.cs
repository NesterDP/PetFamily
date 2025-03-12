using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Species.Commands.Create;

public record CreateSpeciesCommand(string Name) : ICommand;