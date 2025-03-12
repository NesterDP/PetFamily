using PetFamily.Species.Application.SpeciesManagement.Commands.Create;

namespace PetFamily.Species.Presentation.Species.Requests;

public record CreateSpeciesRequest(string Name)
{
    public CreateSpeciesCommand ToCommand() => new CreateSpeciesCommand(Name);
}