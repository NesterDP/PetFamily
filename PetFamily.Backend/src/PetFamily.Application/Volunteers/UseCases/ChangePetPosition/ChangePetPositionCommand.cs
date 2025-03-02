namespace PetFamily.Application.Volunteers.UseCases.ChangePetPosition;

public record ChangePetPositionCommand(Guid VolunteerId, Guid PetId, int Position);