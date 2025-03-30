using PetFamily.Core.Abstractions;

namespace PetFamily.VolunteerRequests.Application.Commands.RejectRequest;

public record RejectRequestCommand(Guid RequestId, Guid AdminId) : ICommand;