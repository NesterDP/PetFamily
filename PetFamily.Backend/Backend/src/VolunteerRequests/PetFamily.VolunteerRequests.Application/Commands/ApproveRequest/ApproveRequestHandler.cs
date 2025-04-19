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

namespace PetFamily.VolunteerRequests.Application.Commands.ApproveRequest;

public class ApproveRequestHandler : ICommandHandler<Guid, ApproveRequestCommand>
{
    private readonly IVolunteerRequestsRepository _volunteerRequestsRepository;
    private readonly ILogger<ApproveRequestHandler> _logger;
    private readonly IValidator<ApproveRequestCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublisher _publisher;

    public ApproveRequestHandler(
        IVolunteerRequestsRepository volunteerRequestsRepository,
        ILogger<ApproveRequestHandler> logger,
        IValidator<ApproveRequestCommand> validator,
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
        ApproveRequestCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToErrorList();

        var requestId = VolunteerRequestId.Create(command.RequestId);

        var adminId = AdminId.Create(command.AdminId);

        using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var request = await _volunteerRequestsRepository
                .GetByIdAsync(requestId, cancellationToken);

            if (request.IsFailure)
                return Errors.General.ValueNotFound($"VolunteerRequest with Id = {requestId.Value}").ToErrorList();

            var result = request.Value.SetApproved(adminId);
            if (result.IsFailure)
                return result.Error.ToErrorList();
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _publisher.PublishDomainEvents(request.Value, cancellationToken);

            transaction.Commit();

            _logger.LogInformation(
                "Admin with ID = {ID1} gave volunteer role to user with id = {ID2}", adminId.Value,
                request.Value.UserId);

            return requestId.Value;
        }
        catch (Exception e)
        {
            transaction.Rollback();
            _logger.LogError(e, "Failed to approve request");
            return Errors.General.Failure("Transaction failed").ToErrorList();
        }
    }
}