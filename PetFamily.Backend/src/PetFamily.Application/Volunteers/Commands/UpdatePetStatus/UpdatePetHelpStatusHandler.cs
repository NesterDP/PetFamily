using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Database;
using PetFamily.Application.Extensions;
using PetFamily.Domain.PetContext.ValueObjects.PetVO;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Application.Volunteers.Commands.UpdatePetStatus;

public class UpdatePetHelpStatusHandler : ICommandHandler<Guid, UpdatePetHelpStatusCommand>
{
    private readonly IValidator<UpdatePetHelpStatusCommand> _validator;
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly ILogger<UpdatePetHelpStatusHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IReadDbContext _readDbContext;

    public UpdatePetHelpStatusHandler(
        IValidator<UpdatePetHelpStatusCommand> validator,
        IVolunteersRepository volunteersRepository,
        ILogger<UpdatePetHelpStatusHandler> logger,
        IUnitOfWork unitOfWork,
        IReadDbContext readDbContext)
    {
        _validator = validator;
        _volunteersRepository = volunteersRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _readDbContext = readDbContext;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        UpdatePetHelpStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToErrorList();


        var volunteerId = VolunteerId.Create(command.VolunteerId);
        var volunteerResult = await _volunteersRepository.GetByIdAsync(volunteerId, cancellationToken);
        if (volunteerResult.IsFailure)
            return volunteerResult.Error.ToErrorList();
        
        Enum.TryParse(command.HelpStatus, out PetStatus status);
        var helpStatus = HelpStatus.Create(status).Value;

        var updateResult = volunteerResult.Value.UpdatePetHelpStatus(
            command.PetId,
            helpStatus);

        if (updateResult.IsFailure)
            return updateResult.Error.ToErrorList();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Successfully updated helpStatus of pet with ID = {ID}",
            command.PetId);

        return command.PetId;
    }
}