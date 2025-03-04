namespace PetFamily.Application.Volunteers.Commands.UpdatePetStatus;

public record UpdatePetHelpStatusCommand(Guid VolunteerId, Guid PetId, string HelpStatus);