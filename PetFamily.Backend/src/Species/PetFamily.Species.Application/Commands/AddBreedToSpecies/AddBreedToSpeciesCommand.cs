using PetFamily.Core.Abstractions;

namespace PetFamily.Species.Application.Commands.AddBreedToSpecies;

public record AddBreedToSpeciesCommand(Guid Id, string Name) : ICommand;