using PetFamily.Core.Abstractions;

namespace PetFamily.Volunteers.Application.VolunteersManagement.Commands.ChangePetPosition;

public record ChangePetPositionCommand(Guid VolunteerId, Guid PetId, int Position) : ICommand;