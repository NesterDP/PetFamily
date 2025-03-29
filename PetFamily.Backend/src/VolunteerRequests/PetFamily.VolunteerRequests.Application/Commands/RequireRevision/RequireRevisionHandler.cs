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
using PetFamily.VolunteerRequests.Domain.ValueObjects;

namespace PetFamily.VolunteerRequests.Application.Commands.RequireRevision;

public class RequireRevisionHandler : ICommandHandler<Guid, RequireRevisionCommand>
{
    private readonly IVolunteerRequestsRepository _volunteerRequestsRepository;
    private readonly ILogger<RequireRevisionHandler> _logger;
    private readonly IValidator<RequireRevisionCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public RequireRevisionHandler(
        IVolunteerRequestsRepository volunteerRequestsRepository,
        ILogger<RequireRevisionHandler> logger,
        IValidator<RequireRevisionCommand> validator,
        [FromKeyedServices(UnitOfWorkSelector.VolunteerRequests)]
        IUnitOfWork unitOfWork)
    {
        _volunteerRequestsRepository = volunteerRequestsRepository;
        _logger = logger;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        RequireRevisionCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToErrorList();

        var requestId = VolunteerRequestId.Create(command.RequestId);

        var adminId = AdminId.Create(command.AdminId);
        
        var revisionComment = RevisionComment.Create(command.RevisionComment).Value;

        var request = await _volunteerRequestsRepository
            .GetByIdAsync(requestId, cancellationToken);

        if (request.IsFailure)
            return Errors.General.ValueNotFound($"VolunteerRequest with Id = {requestId.Value}").ToErrorList();

        var result = request.Value.SetRevisionRequired(adminId, revisionComment);
        if (result.IsFailure)
            return result.Error.ToErrorList();
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Admin with ID = {ID1} required revision of review with Id = {ID2}", adminId.Value, requestId.Value);

        return requestId.Value;
    }
}