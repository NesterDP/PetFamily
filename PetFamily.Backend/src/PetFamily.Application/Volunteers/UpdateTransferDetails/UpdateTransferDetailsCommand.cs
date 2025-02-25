using PetFamily.Application.Dto.Shared;

namespace PetFamily.Application.Volunteers.UpdateTransferDetails;

public record UpdateTransferDetailsCommand(
    Guid Id,
    TransferDetailsDto Dto);