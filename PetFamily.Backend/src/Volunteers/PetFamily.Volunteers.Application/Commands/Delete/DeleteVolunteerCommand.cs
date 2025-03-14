using PetFamily.Core.Abstractions;

namespace PetFamily.Volunteers.Application.Commands.Delete;

public record DeleteVolunteerCommand(Guid Id) : ICommand;