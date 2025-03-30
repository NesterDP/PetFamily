using PetFamily.Core.Abstractions;

namespace PetFamily.VolunteerRequests.Application.Commands.ApproveRequest;

public record ApproveRequestCommand(Guid RequestId, Guid AdminId) : ICommand;