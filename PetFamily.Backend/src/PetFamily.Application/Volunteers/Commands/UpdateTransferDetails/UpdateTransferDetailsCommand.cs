using PetFamily.Application.Dto.Shared;

namespace PetFamily.Application.Volunteers.Commands.UpdateTransferDetails;

public record UpdateTransferDetailsCommand(
    Guid Id,
    List<TransferDetailDto> TransferDetailDtos);