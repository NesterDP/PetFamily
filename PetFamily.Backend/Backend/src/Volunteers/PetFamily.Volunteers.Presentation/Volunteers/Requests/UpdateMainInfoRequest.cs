using PetFamily.Core.Dto.Volunteer;
using PetFamily.Volunteers.Application.Commands.UpdateMainInfo;

namespace PetFamily.Volunteers.Presentation.Volunteers.Requests;

public record UpdateMainInfoRequest(
    FullNameDto FullNameDto,
    string Email,
    int Experience,
    string PhoneNumber)
{
    public UpdateMainInfoCommand ToCommand(Guid id) => new(
        id,
        FullNameDto,
        Email,
        Experience,
        PhoneNumber);
}