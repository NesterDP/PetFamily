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

namespace PetFamily.VolunteerRequests.Application.Commands.RejectRequest;

public class RejectRequestHandler : ICommandHandler<Guid, RejectRequestCommand>
{
    private readonly IVolunteerRequestsRepository _volunteerRequestsRepository;
    private readonly ILogger<RejectRequestHandler> _logger;
    private readonly IValidator<RejectRequestCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublisher _publisher;

    public RejectRequestHandler(
        IVolunteerRequestsRepository volunteerRequestsRepository,
        ILogger<RejectRequestHandler> logger,
        IValidator<RejectRequestCommand> validator,
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
        RejectRequestCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToErrorList();

        var requestId = VolunteerRequestId.Create(command.RequestId);

        var adminId = AdminId.Create(command.AdminId);
        
        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var request = await _volunteerRequestsRepository
                .GetByIdAsync(requestId, cancellationToken);

            if (request.IsFailure)
                return Errors.General.ValueNotFound($"VolunteerRequest with Id = {requestId.Value}").ToErrorList();

            var result = request.Value.SetRejected(adminId);
            if (result.IsFailure)
                return result.Error.ToErrorList();
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        
            await _publisher.PublishDomainEvents(request.Value, cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        
            _logger.LogInformation(
                "Admin with ID = {ID1} rejected request with ID = {ID2}", adminId.Value, requestId.Value);

            return requestId.Value;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(e, "Failed to reject request");
            return Errors.General.Failure("Transaction failed").ToErrorList();
        }
    }
}