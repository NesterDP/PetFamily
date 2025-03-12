using PetFamily.Core.Abstractions;

namespace PetFamily.Species.Application.SpeciesManagement.Commands.AddBreedToSpecies;

public record AddBreedToSpeciesCommand(Guid Id, string Name) : ICommand;