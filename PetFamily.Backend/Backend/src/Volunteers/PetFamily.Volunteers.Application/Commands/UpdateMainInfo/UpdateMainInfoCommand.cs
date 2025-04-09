using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Volunteer;

namespace PetFamily.Volunteers.Application.Commands.UpdateMainInfo;

public record UpdateMainInfoCommand(Guid Id,
    FullNameDto FullNameDto,
    string Email,
    string Description,
    int Experience,
    string PhoneNumber) : ICommand;
    