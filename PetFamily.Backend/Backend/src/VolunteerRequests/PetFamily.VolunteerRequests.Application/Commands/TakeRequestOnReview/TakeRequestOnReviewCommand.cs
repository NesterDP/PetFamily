using PetFamily.Core.Abstractions;

namespace PetFamily.VolunteerRequests.Application.Commands.TakeRequestOnReview;

public record TakeRequestOnReviewCommand(Guid RequestId, Guid AdminId) : ICommand;