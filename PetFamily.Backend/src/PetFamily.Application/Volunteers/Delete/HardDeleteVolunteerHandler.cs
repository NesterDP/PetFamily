using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Extensions;
using PetFamily.Application.Volunteers.UpdateMainInfo;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared.CustomErrors;

namespace PetFamily.Application.Volunteers.Delete;

public class HardDeleteVolunteerHandler
{
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly ILogger<HardDeleteVolunteerHandler> _logger;
    private readonly IValidator<DeleteVolunteerCommand> _validator;

    public HardDeleteVolunteerHandler(
        IVolunteersRepository volunteersRepository,
        ILogger<HardDeleteVolunteerHandler> logger,
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
        
        await _volunteersRepository.DeleteAsync(volunteerResult.Value, cancellationToken);
        
        _logger.LogInformation("Volunteer was hard deleted, his ID = {ID}", volunteerId.Value);

        return volunteerId.Value;
    }
}