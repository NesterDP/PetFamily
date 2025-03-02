using PetFamily.Application.Volunteers.Commands.Delete;

namespace PetFamily.API.Controllers.Volunteers.Requests;

public record DeleteVolunteerRequest()
{
    public DeleteVolunteerCommand ToCommand(Guid id) => new(id);
}