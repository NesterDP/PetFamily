using PetFamily.Core.Abstractions;

namespace PetFamily.VolunteerRequests.Application.Commands.RequireRevision;

public record RequireRevisionCommand(Guid RequestId, Guid AdminId, string RevisionComment) : ICommand;
