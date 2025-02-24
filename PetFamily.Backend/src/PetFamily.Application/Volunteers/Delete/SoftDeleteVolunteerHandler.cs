using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Extensions;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared.CustomErrors;

namespace PetFamily.Application.Volunteers.Delete;

public class SoftDeleteVolunteerHandler
{
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly ILogger<SoftDeleteVolunteerHandler> _logger;
    private readonly IValidator<DeleteVolunteerCommand> _validator;

    public SoftDeleteVolunteerHandler(
        IVolunteersRepository volunteersRepository,
        ILogger<SoftDeleteVolunteerHandler> logger,
        IValidator<DeleteVolunteerCommand> validator)
    {
        _volunteersRepository = volunteersRepository;
        _logger = logger;
        _validator = validator;
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
        await _volunteersRepository.SaveAsync(volunteerResult.Value, cancellationToken);

        _logger.LogInformation("Volunteer was soft deleted, his ID = {ID}", volunteerId.Value);

        return volunteerId.Value;
    }
}