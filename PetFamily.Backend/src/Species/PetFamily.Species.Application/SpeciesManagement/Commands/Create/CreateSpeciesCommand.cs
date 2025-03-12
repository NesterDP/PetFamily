using PetFamily.Core.Abstractions;

namespace PetFamily.Species.Application.SpeciesManagement.Commands.Create;

public record CreateSpeciesCommand(string Name) : ICommand;