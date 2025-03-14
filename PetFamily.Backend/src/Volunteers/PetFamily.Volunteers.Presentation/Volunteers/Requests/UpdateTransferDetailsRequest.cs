using PetFamily.Core.Dto.Shared;
using PetFamily.Volunteers.Application.Commands.UpdateTransferDetails;

namespace PetFamily.Volunteers.Presentation.Volunteers.Requests;

public record UpdateTransferDetailsRequest(List<TransferDetailDto> TransferDetailsDto)
{
    public UpdateTransferDetailsCommand ToCommand(Guid id) => new(id, TransferDetailsDto);
}