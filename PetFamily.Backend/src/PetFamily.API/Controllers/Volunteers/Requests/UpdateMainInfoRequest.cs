using PetFamily.Application.Dto.Volunteer;
using PetFamily.Application.Volunteers.UpdateMainInfo;

namespace PetFamily.API.Controllers.Volunteers.Requests;

public record UpdateMainInfoRequest(
    FullNameDto FullNameDto,
    string Email,
    string Description,
    int Experience,
    string PhoneNumber)
{
    public UpdateMainInfoCommand ToCommand(Guid id) => new(
        id,
        FullNameDto,
        Email,
        Description,
        Experience,
        PhoneNumber);
}
    