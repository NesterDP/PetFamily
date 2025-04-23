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
using PetFamily.VolunteerRequests.Domain.ValueObjects;

namespace PetFamily.VolunteerRequests.Application.Commands.AmendRequest;

public class AmendRequestHandler : ICommandHandler<Guid, AmendRequestCommand>
{
    private readonly IVolunteerRequestsRepository _volunteerRequestsRepository;
    private readonly ILogger<AmendRequestHandler> _logger;
    private readonly IValidator<AmendRequestCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublisher _publisher;

    public AmendRequestHandler(
        IVolunteerRequestsRepository volunteerRequestsRepository,
        ILogger<AmendRequestHandler> logger,
        IValidator<AmendRequestCommand> validator,
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
        AmendRequestCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToErrorList();

        var requestId = VolunteerRequestId.Create(command.RequestId);

        var userId = UserId.Create(command.UserId);

        var updatedInfo = VolunteerInfo.Create(command.UpdatedInfo).Value;
        
        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var request = await _volunteerRequestsRepository
                .GetByIdAsync(requestId, cancellationToken);

            if (request.IsFailure)
                return Errors.General.ValueNotFound($"VolunteerRequest with Id = {requestId.Value}").ToErrorList();

            if (request.Value.UserId != userId)
                return Errors.General
                    .Conflict($"User with id = {userId.Value} cannot amend request with id = {requestId.Value}")
                    .ToErrorList();

            var result = request.Value.SetSubmitted(updatedInfo);
            if (result.IsFailure)
                return result.Error.ToErrorList();
        
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _publisher.PublishDomainEvents(request.Value, cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "User with ID = {ID1} amended request with Id = {ID2}", userId.Value, requestId.Value);

            return requestId.Value;
        }
        
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(e, "Failed to amend request");
            return Errors.General.Failure("Transaction failed").ToErrorList();
        }
    }
}