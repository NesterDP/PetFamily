using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Database;
using PetFamily.Application.Extensions;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Application.Volunteers.Commands.UpdateTransferDetails;

public class UpdateTransferDetailsHandler : ICommandHandler<Guid, UpdateTransferDetailsCommand>
{
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly ILogger<UpdateTransferDetailsHandler> _logger;
    private readonly IValidator<UpdateTransferDetailsCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTransferDetailsHandler(
        IVolunteersRepository volunteersRepository,
        ILogger<UpdateTransferDetailsHandler> logger,
        IValidator<UpdateTransferDetailsCommand> validator,
        IUnitOfWork unitOfWork)
    {
        _volunteersRepository = volunteersRepository;
        _logger = logger;
        _validator = validator;
        _unitOfWork = unitOfWork;
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
        foreach (var transferDetail in command.TransferDetailDtos)
        {
            var tempResult = TransferDetail.Create(transferDetail.Name, transferDetail.Description);
            transferDetailsList.Add(tempResult.Value);
        }

        volunteerResult.Value.UpdateTransferDetails(transferDetailsList);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Volunteer was updated (transfer details), his ID = {ID}", volunteerId.Value);

        return volunteerId.Value;
    }
}