using PetFamily.Application.Dto.Shared;
using PetFamily.Application.Volunteers.UpdateTransferDetails;

namespace PetFamily.API.Controllers.Volunteers.Requests;

public record UpdateTransferDetailsRequest(TransferDetailsDto TransferDetailsDto)
{
    public UpdateTransferDetailsCommand ToCommand(Guid id) => new(id, TransferDetailsDto);
}