using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.Volunteers.Domain.ValueObjects.PetVO;
using PetFamily.Volunteers.Domain.ValueObjects.VolunteerVO;

namespace PetFamily.Volunteers.Application.Commands.ChangePetPosition;

public class ChangePetPositionHandler : ICommandHandler<Guid, ChangePetPositionCommand>
{
    private readonly IValidator<ChangePetPositionCommand> _validator;
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly ILogger<ChangePetPositionHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public ChangePetPositionHandler(
        IValidator<ChangePetPositionCommand> validator,
        IVolunteersRepository volunteersRepository,
        ILogger<ChangePetPositionHandler> logger,
        [FromKeyedServices("volunteer")] IUnitOfWork unitOfWork)
    {
        _validator = validator;
        _volunteersRepository = volunteersRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        ChangePetPositionCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToErrorList();

        var volunteerId = VolunteerId.Create(command.VolunteerId);
        var volunteerResult = await _volunteersRepository.GetByIdAsync(volunteerId, cancellationToken);
        if (volunteerResult.IsFailure)
            return volunteerResult.Error.ToErrorList();

        var petId = PetId.Create(command.PetId);
        var pet = volunteerResult.Value.GetPetById(petId);
        if (pet.IsFailure)
            return pet.Error.ToErrorList();

        var position = Position.Create(command.Position).Value;

        var positionResult = volunteerResult.Value.MovePet(pet.Value, position);
        if (positionResult.IsFailure)
            return positionResult.Error.ToErrorList();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("volunteer with Id = {ID} moved pet with ID = {PID} to position = {P}",
            volunteerId.Value, petId.Value, position.Value);

        return petId.Value;
    }
}