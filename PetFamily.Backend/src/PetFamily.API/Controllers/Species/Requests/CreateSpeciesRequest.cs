using PetFamily.Application.Species.Commands.Create;

namespace PetFamily.API.Controllers.Species.Requests;

public record CreateSpeciesRequest(string Name)
{
    public CreateSpeciesCommand ToCommand() => new CreateSpeciesCommand(Name);
}