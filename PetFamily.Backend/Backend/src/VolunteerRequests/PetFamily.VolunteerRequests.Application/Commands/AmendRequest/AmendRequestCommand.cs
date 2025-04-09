using PetFamily.Core.Abstractions;

namespace PetFamily.VolunteerRequests.Application.Commands.AmendRequest;

public record AmendRequestCommand(Guid RequestId, Guid UserId, string UpdatedInfo) : ICommand;
