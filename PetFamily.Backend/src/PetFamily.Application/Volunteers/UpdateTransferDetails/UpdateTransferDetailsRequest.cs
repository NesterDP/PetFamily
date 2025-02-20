using PetFamily.Application.Dto.Shared;

namespace PetFamily.Application.Volunteers.UpdateTransferDetails;

public record UpdateTransferDetailsRequest(
    Guid Id,
    TransferDetailsDto Dto);