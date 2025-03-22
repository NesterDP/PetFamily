using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Shared;
using PetFamily.Core.Dto.Volunteer;

namespace PetFamily.Volunteers.Application.Commands.Create;

public record CreateVolunteerCommand(CreateVolunteerDto CreateVolunteerDto) : ICommand;