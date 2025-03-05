using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Volunteers.Commands.ChangePetPosition;

public record ChangePetPositionCommand(Guid VolunteerId, Guid PetId, int Position) : ICommand;