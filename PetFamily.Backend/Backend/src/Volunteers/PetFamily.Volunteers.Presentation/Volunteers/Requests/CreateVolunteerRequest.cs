using PetFamily.Core.Dto.Shared;
using PetFamily.Core.Dto.Volunteer;
using PetFamily.Volunteers.Application.Commands.Create;

namespace PetFamily.Volunteers.Presentation.Volunteers.Requests;

public record CreateVolunteerRequest(CreateVolunteerDto CreateVolunteerDto)
{
    public CreateVolunteerCommand ToCommand() => new(CreateVolunteerDto);
}