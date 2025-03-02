using PetFamily.Application.Dto.Shared;

namespace PetFamily.Application.Volunteers.UseCases.UpdateTransferDetails;

public record UpdateTransferDetailsCommand(
    Guid Id,
    TransferDetailsDto Dto);