using PetFamily.Application.Species.Commands.AddBreedToSpecies;

namespace PetFamily.API.Controllers.Species.Requests;

public record AddBreedToSpeciesRequest(string Name)
{
    public AddBreedToSpeciesCommand ToCommand(Guid id) => new AddBreedToSpeciesCommand(id, Name);
}