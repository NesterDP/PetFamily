using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.Structs;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;
using PetFamily.Volunteers.Domain.ValueObjects.VolunteerVO;

namespace PetFamily.Volunteers.Application.Commands.UpdateTransferDetails;

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
        [FromKeyedServices(UnitOfWorkSelector.Volunteers)] IUnitOfWork unitOfWork)
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