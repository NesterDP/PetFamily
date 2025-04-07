using PetFamily.Core.Abstractions;

namespace PetFamily.Species.Application.Commands.DeleteBreedById;

public record DeleteBreedByIdCommand(Guid SpeciesId, Guid BreedId) : ICommand;