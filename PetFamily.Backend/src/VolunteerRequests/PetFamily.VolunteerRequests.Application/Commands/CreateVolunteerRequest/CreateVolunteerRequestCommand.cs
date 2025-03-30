using PetFamily.Core.Abstractions;

namespace PetFamily.VolunteerRequests.Application.Commands.CreateVolunteerRequest;

public record CreateVolunteerRequestCommand(Guid UserId, string VolunteerInfo) : ICommand;