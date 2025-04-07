using PetFamily.Core.Abstractions;

namespace PetFamily.Volunteers.Application.Commands.ChangePetPosition;

public record ChangePetPositionCommand(Guid VolunteerId, Guid PetId, int Position) : ICommand;