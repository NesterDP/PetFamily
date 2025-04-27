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

namespace PetFamily.Volunteers.Application.Commands.UpdatePetMainPhoto;

public class UpdatePetMainPhotoHandler : ICommandHandler<Guid, UpdatePetMainPhotoCommand>
{
    private readonly IValidator<UpdatePetMainPhotoCommand> _validator;
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly ILogger<UpdatePetMainPhotoHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublisher _publisher;

    public UpdatePetMainPhotoHandler(
        IValidator<UpdatePetMainPhotoCommand> validator,
        IVolunteersRepository volunteersRepository,
        ILogger<UpdatePetMainPhotoHandler> logger,
        [FromKeyedServices(UnitOfWorkSelector.Volunteers)]
        IUnitOfWork unitOfWork,
        IPublisher publisher)
    {
        _validator = validator;
        _volunteersRepository = volunteersRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _publisher = publisher;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        UpdatePetMainPhotoCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToErrorList();

        var volunteerId = VolunteerId.Create(command.VolunteerId);
        var volunteerResult = await _volunteersRepository.GetByIdAsync(volunteerId, cancellationToken);
        if (volunteerResult.IsFailure)
            return volunteerResult.Error.ToErrorList();

        var updateResult = volunteerResult.Value.UpdatePetMainPhoto(command.PetId, command.MainPhotoId);
        if (updateResult.IsFailure)
            return updateResult.Error.ToErrorList();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _publisher.Publish(new PetWasChangedEvent(), cancellationToken);

        _logger.LogInformation("Successfully updated main photo of pet with ID = {ID}", command.PetId);

        return command.PetId;
    }
}