using PetFamily.Application.Dto.Shared;
using PetFamily.Application.Volunteers.UseCases.UpdateTransferDetails;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.API.Controllers.Volunteers.Requests;

public record UpdateTransferDetailsRequest(List<TransferDetailDto> TransferDetailsDto)
{
    public UpdateTransferDetailsCommand ToCommand(Guid id) => new(id, TransferDetailsDto);
}