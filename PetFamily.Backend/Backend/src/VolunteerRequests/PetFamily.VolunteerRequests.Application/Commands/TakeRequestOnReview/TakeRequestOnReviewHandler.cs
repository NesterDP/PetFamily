using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Extensions;
using PetFamily.Discussions.Contracts;
using PetFamily.Discussions.Contracts.Requests;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.Structs;
using PetFamily.SharedKernel.ValueObjects.Ids;
using PetFamily.VolunteerRequests.Application.Abstractions;

namespace PetFamily.VolunteerRequests.Application.Commands.TakeRequestOnReview;

public class TakeRequestOnReviewHandler : ICommandHandler<Guid, TakeRequestOnReviewCommand>
{
    private readonly IVolunteerRequestsRepository _volunteerRequestsRepository;
    private readonly ILogger<TakeRequestOnReviewHandler> _logger;
    private readonly IValidator<TakeRequestOnReviewCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICreateDiscussionContract _contract;

    public TakeRequestOnReviewHandler(
        IVolunteerRequestsRepository volunteerRequestsRepository,
        ILogger<TakeRequestOnReviewHandler> logger,
        IValidator<TakeRequestOnReviewCommand> validator,
        [FromKeyedServices(UnitOfWorkSelector.VolunteerRequests)]
        IUnitOfWork unitOfWork,
        ICreateDiscussionContract contract)
    {
        _volunteerRequestsRepository = volunteerRequestsRepository;
        _logger = logger;
        _validator = validator;
        _unitOfWork = unitOfWork;
        _contract = contract;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        TakeRequestOnReviewCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToErrorList();

        var adminId = AdminId.Create(command.AdminId);

        var requestId = VolunteerRequestId.Create(command.RequestId);

        var request = await _volunteerRequestsRepository
            .GetByIdAsync(requestId, cancellationToken);

        if (request.IsFailure)
            return Errors.General.ValueNotFound($"VolunteerRequest with Id = {requestId.Value}").ToErrorList();

        var result = request.Value.SetOnReview(adminId);
        if (result.IsFailure)
            return result.Error.ToErrorList();
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        // будет отправлено в брокер
        if (request.Value.RevisionComment == null)
        {
            List<Guid> userIds = [adminId, request.Value.UserId];
            var contractRequest = new CreateDiscussionRequest(requestId, userIds);
            var discussion = await _contract.CreateDiscussion(contractRequest, cancellationToken);
            //if (discussion.IsFailure)
                //return discussion.Error;
        }

        _logger.LogInformation(
            "Admin with ID = {ID1} took request with {ID2} on review", adminId.Value, requestId.Value);

        return requestId.Value;
    }
}