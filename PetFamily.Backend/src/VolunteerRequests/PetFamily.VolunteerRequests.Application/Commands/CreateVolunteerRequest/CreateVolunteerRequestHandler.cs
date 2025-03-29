using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.Structs;
using PetFamily.SharedKernel.ValueObjects.Ids;
using PetFamily.VolunteerRequests.Application.Abstractions;
using PetFamily.VolunteerRequests.Domain.Entities;
using PetFamily.VolunteerRequests.Domain.ValueObjects;

namespace PetFamily.VolunteerRequests.Application.Commands.CreateVolunteerRequest;

public class CreateVolunteerRequestHandler : ICommandHandler<Guid, CreateVolunteerRequestCommand>
{
    private readonly IVolunteerRequestsRepository _volunteerRequestsRepository;
    private readonly ILogger<CreateVolunteerRequestHandler> _logger;
    private readonly IValidator<CreateVolunteerRequestCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly int BAN_DURATION_IN_DAYS = 7;

    public CreateVolunteerRequestHandler(
        IVolunteerRequestsRepository volunteerRequestsRepository,
        ILogger<CreateVolunteerRequestHandler> logger,
        IValidator<CreateVolunteerRequestCommand> validator,
        [FromKeyedServices(UnitOfWorkSelector.VolunteerRequests)]
        IUnitOfWork unitOfWork)
    {
        _volunteerRequestsRepository = volunteerRequestsRepository;
        _logger = logger;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        CreateVolunteerRequestCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToErrorList();

        var userId = UserId.Create(command.UserId);

        var existedRequests = await _volunteerRequestsRepository
            .GetRequestsByUserIdAsync(userId, cancellationToken);

        var checkResult = CheckExistedRequests(existedRequests);
        if (checkResult.IsFailure)
            return checkResult.Error.ToErrorList();

        var volunteerInfo = VolunteerInfo.Create(command.VolunteerInfo).Value;

        var volunteerRequest = new VolunteerRequest(userId, volunteerInfo);

        await _volunteerRequestsRepository.AddAsync(volunteerRequest, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Created VolunteerRequest with ID = {id}", volunteerRequest.Id.Value);

        return volunteerRequest.Id.Value;
    }

    private UnitResult<Error> CheckExistedRequests(
        List<VolunteerRequest> requests)
    {
        var forbiddenStatuses = new[]
        {
            VolunteerRequestStatusEnum.Submitted,
            VolunteerRequestStatusEnum.OnReview,
            VolunteerRequestStatusEnum.RevisionRequired
        };

        if (requests.Any(r => forbiddenStatuses.Contains(r.Status.Value)))
            return Errors.General.AlreadyExist(
                $"Active volunteer request for user with Id = {requests[0].UserId.Value}");


        if (!requests.Any(r => r.RejectedAt.HasValue))
        {
            return UnitResult.Success<Error>();
        }

        var banEndDate = requests.Max(r => r.RejectedAt!.Value).AddDays(BAN_DURATION_IN_DAYS);
        if (banEndDate > DateTime.UtcNow)
            return Errors.General.Conflict(
                $"User with Id = {requests[0].UserId.Value} isn't allowed to submit new requests until {banEndDate}");

        return UnitResult.Success<Error>();
    }
}