using PetFamily.Core.Abstractions;

namespace PetFamily.Species.Application.SpeciesManagement.Commands.DeleteBreedById;

public record DeleteBreedByIdCommand(Guid SpeciesId, Guid BreedId) : ICommand;