using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Extensions;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Application.Volunteers.UpdateTransferDetails;

public class UpdateTransferDetailsHandler
{
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly ILogger<UpdateTransferDetailsHandler> _logger;
    private readonly IValidator<UpdateTransferDetailsCommand> _validator;

    public UpdateTransferDetailsHandler(
        IVolunteersRepository volunteersRepository,
        ILogger<UpdateTransferDetailsHandler> logger,
        IValidator<UpdateTransferDetailsCommand> validator)
    {
        _volunteersRepository = volunteersRepository;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        UpdateTransferDetailsCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToErrorList();
        
        var volunteerId = VolunteerId.Create(command.Id);

        var volunteerResult = await _volunteersRepository.GetByIdAsync(volunteerId, cancellationToken);
        if (volunteerResult.IsFailure)
            return volunteerResult.Error.ToErrorList();

        List<TransferDetail> transferDetailsList = [];
        foreach (var transferDetail in command.Dto.TransferDetails)
        {
            var tempResult = TransferDetail.Create(transferDetail.Name, transferDetail.Description);
            transferDetailsList.Add(tempResult.Value);
        }

        volunteerResult.Value.UpdateTransferDetails(TransferDetailsList.Create(transferDetailsList).Value);

        var result = await _volunteersRepository.SaveAsync(volunteerResult.Value, cancellationToken);

        _logger.LogInformation(
            "Volunteer was updated (transfer details), his ID = {ID}", volunteerId.Value);

        return volunteerId.Value;
    }
}