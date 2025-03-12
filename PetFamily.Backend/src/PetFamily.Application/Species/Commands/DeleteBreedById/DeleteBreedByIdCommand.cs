using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Species.Commands.DeleteBreedById;

public record DeleteBreedByIdCommand(Guid SpeciesId, Guid BreedId) : ICommand;