using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Accounts.Contracts;
using PetFamily.Accounts.Contracts.Requests;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.CustomErrors;
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
    private readonly ICreateVolunteerAccountContract _contract;

    public ApproveRequestHandler(
        IVolunteerRequestsRepository volunteerRequestsRepository,
        ILogger<ApproveRequestHandler> logger,
        IValidator<ApproveRequestCommand> validator,
        [FromKeyedServices(UnitOfWorkSelector.VolunteerRequests)]
        IUnitOfWork unitOfWork,
        ICreateVolunteerAccountContract contract)
    {
        _volunteerRequestsRepository = volunteerRequestsRepository;
        _logger = logger;
        _validator = validator;
        _unitOfWork = unitOfWork;
        _contract = contract;
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
        
        var request = await _volunteerRequestsRepository
            .GetByIdAsync(requestId, cancellationToken);

        if (request.IsFailure)
            return Errors.General.ValueNotFound($"VolunteerRequest with Id = {requestId.Value}").ToErrorList();
        
        var result = request.Value.SetApproved(adminId);
        if (result.IsFailure)
            return result.Error.ToErrorList();

        var createVolunteerAccountRequest = new CreateVolunteerAccountRequest(request.Value.UserId);
        var accountResult = await _contract.CreateVolunteerAccountAsync(createVolunteerAccountRequest, cancellationToken);
        if (accountResult.IsFailure)
            return accountResult.Error.ToErrorList();
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Admin with ID = {ID1} gave volunteer role to user with id = {ID2}", adminId.Value, request.Value.UserId);

        return requestId.Value;
    }
}