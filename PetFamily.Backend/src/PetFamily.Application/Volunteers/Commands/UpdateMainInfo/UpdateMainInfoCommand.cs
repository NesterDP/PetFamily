using PetFamily.Application.Abstractions;
using PetFamily.Application.Dto.Volunteer;

namespace PetFamily.Application.Volunteers.Commands.UpdateMainInfo;

public record UpdateMainInfoCommand(Guid Id,
    FullNameDto FullNameDto,
    string Email,
    string Description,
    int Experience,
    string PhoneNumber) : ICommand;
    