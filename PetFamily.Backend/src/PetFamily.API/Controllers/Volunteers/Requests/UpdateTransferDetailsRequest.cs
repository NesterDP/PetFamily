using PetFamily.Application.Dto.Shared;

namespace PetFamily.API.Controllers.Volunteers.Requests;

public record UpdateTransferDetailsRequest(
    Guid Id,
    TransferDetailsDto TransferDetailsDto);