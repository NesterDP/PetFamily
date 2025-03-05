using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Volunteers.Commands.Delete;

public record DeleteVolunteerCommand(Guid Id) : ICommand;