using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.Extensions;
using PetFamily.SharedKernel.Structs;
using PetFamily.SharedKernel.ValueObjects.Ids;
using PetFamily.VolunteerRequests.Application.Abstractions;
using PetFamily.VolunteerRequests.Domain.Events;

namespace PetFamily.VolunteerRequests.Application.Commands.TakeRequestOnReview;

public class TakeRequestOnReviewHandler : ICommandHandler<Guid, TakeRequestOnReviewCommand>
{
    private readonly IVolunteerRequestsRepository _volunteerRequestsRepository;
    private readonly ILogger<TakeRequestOnReviewHandler> _logger;
    private readonly IValidator<TakeRequestOnReviewCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublisher _publisher;

    public TakeRequestOnReviewHandler(
        IVolunteerRequestsRepository volunteerRequestsRepository,
        ILogger<TakeRequestOnReviewHandler> logger,
        IValidator<TakeRequestOnReviewCommand> validator,
        [FromKeyedServices(UnitOfWorkSelector.VolunteerRequests)]
        IUnitOfWork unitOfWork,
        IPublisher publisher)
    {
        _volunteerRequestsRepository = volunteerRequestsRepository;
        _logger = logger;
        _validator = validator;
        _unitOfWork = unitOfWork;
        _publisher = publisher;
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

        using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var request = await _volunteerRequestsRepository
                .GetByIdAsync(requestId, cancellationToken);

            if (request.IsFailure)
                return Errors.General.ValueNotFound($"VolunteerRequest with Id = {requestId.Value}").ToErrorList();

            var result = request.Value.SetOnReview(adminId);
            if (result.IsFailure)
                return result.Error.ToErrorList();

            if (request.Value.RevisionComment is not null)
                request.Value.RemoveAllDomainEvents<VolunteerRequestWasTakenOnReviewEvent>();
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _publisher.PublishDomainEvents(request.Value, cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "Admin with ID = {ID1} took request with {ID2} on review", adminId.Value, requestId.Value);

            return requestId.Value;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(e, "Failed to take request on review");
            return Errors.General.Failure("Transaction failed").ToErrorList();
        }
    }
}