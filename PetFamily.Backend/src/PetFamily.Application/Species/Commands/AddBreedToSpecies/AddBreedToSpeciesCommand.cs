using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Species.Commands.AddBreedToSpecies;

public record AddBreedToSpeciesCommand(Guid Id, string Name) : ICommand;