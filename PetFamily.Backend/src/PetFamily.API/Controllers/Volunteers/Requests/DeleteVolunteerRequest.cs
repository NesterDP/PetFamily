using PetFamily.Application.Volunteers.UseCases.Delete;

namespace PetFamily.API.Controllers.Volunteers.Requests;

public record DeleteVolunteerRequest()
{
    public DeleteVolunteerCommand ToCommand(Guid id) => new(id);
}