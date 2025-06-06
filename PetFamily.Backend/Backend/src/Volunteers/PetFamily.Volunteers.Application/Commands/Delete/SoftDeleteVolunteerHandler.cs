using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.Structs;
using PetFamily.SharedKernel.ValueObjects.Ids;
using PetFamily.Volunteers.Domain.Events;

namespace PetFamily.Volunteers.Application.Commands.Delete;

public class SoftDeleteVolunteerHandler : ICommandHandler<Guid, DeleteVolunteerCommand>
{
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly ILogger<SoftDeleteVolunteerHandler> _logger;
    private readonly IValidator<DeleteVolunteerCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublisher _publisher;

    public SoftDeleteVolunteerHandler(
        IVolunteersRepository volunteersRepository,
        ILogger<SoftDeleteVolunteerHandler> logger,
        IValidator<DeleteVolunteerCommand> validator,
        [FromKeyedServices(UnitOfWorkSelector.Volunteers)]
        IUnitOfWork unitOfWork,
        IPublisher publisher)
    {
        _volunteersRepository = volunteersRepository;
        _logger = logger;
        _validator = validator;
        _unitOfWork = unitOfWork;
        _publisher = publisher;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        DeleteVolunteerCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToErrorList();

        var volunteerId = VolunteerId.Create(command.Id);

        var volunteerResult = await _volunteersRepository.GetByIdAsync(volunteerId, cancellationToken);
        if (volunteerResult.IsFailure)
            return volunteerResult.Error.ToErrorList();

        volunteerResult.Value.Delete();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _publisher.Publish(new PetWasChangedEvent(), cancellationToken);

        _logger.LogInformation("Volunteer was soft deleted, his ID = {ID}", volunteerId.Value);

        return volunteerId.Value;
    }
}