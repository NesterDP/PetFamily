using PetFamily.Application.Dto.Shared;
using PetFamily.Application.Volunteers.Commands.UpdateTransferDetails;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.API.Controllers.Volunteers.Requests;

public record UpdateTransferDetailsRequest(List<TransferDetailDto> TransferDetailsDto)
{
    public UpdateTransferDetailsCommand ToCommand(Guid id) => new(id, TransferDetailsDto);
}