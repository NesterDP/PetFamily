using PetFamily.Core.Abstractions;

namespace PetFamily.Volunteers.Application.VolunteersManagement.Commands.Delete;

public record DeleteVolunteerCommand(Guid Id) : ICommand;