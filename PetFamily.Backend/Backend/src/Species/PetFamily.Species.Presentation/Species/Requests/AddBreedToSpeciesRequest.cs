using PetFamily.Species.Application.Commands.AddBreedToSpecies;

namespace PetFamily.Species.Presentation.Species.Requests;

public record AddBreedToSpeciesRequest(string Name)
{
    public AddBreedToSpeciesCommand ToCommand(Guid id) => new AddBreedToSpeciesCommand(id, Name);
}