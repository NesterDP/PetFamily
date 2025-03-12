using PetFamily.Core.Abstractions;

namespace PetFamily.Volunteers.Application.VolunteersManagement.Commands.UpdatePetStatus;

public record UpdatePetHelpStatusCommand(Guid VolunteerId, Guid PetId, string HelpStatus) : ICommand;