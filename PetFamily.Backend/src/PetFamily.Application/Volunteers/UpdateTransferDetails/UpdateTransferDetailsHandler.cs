using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Application.Volunteers.UpdateTransferDetails;

public class UpdateTransferDetailsHandler
{
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly ILogger<UpdateTransferDetailsHandler> _logger;

    public UpdateTransferDetailsHandler(
        IVolunteersRepository volunteersRepository,
        ILogger<UpdateTransferDetailsHandler> logger)
    {
        _volunteersRepository = volunteersRepository;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> HandleAsync(
        UpdateTransferDetailsRequest request,
        CancellationToken cancellationToken)
    {
        var volunteerId = VolunteerId.Create(request.Id);

        var volunteerResult = await _volunteersRepository.GetByIdAsync(volunteerId, cancellationToken);
        if (volunteerResult.IsFailure)
            return volunteerResult.Error;

        List<TransferDetail> transferDetailList = [];
        foreach (var transferDetail in request.Dto.TransferDetails)
        {
            var tempResult = TransferDetail.Create(transferDetail.Name, transferDetail.Description);
            transferDetailList.Add(tempResult.Value);
        }

        volunteerResult.Value.UpdateTransferDetails(TransferDetailList.Create(transferDetailList).Value);

        var result = await _volunteersRepository.SaveAsync(volunteerResult.Value, cancellationToken);

        _logger.LogInformation(
            "Volunteer was updated (transfer details), his ID = {ID}", volunteerId.Value);

        return volunteerId.Value;
    }
}