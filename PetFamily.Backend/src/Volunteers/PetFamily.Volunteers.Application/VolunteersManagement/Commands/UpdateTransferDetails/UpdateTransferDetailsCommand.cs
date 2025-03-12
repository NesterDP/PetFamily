using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Shared;

namespace PetFamily.Volunteers.Application.VolunteersManagement.Commands.UpdateTransferDetails;

public record UpdateTransferDetailsCommand(
    Guid Id,
    List<TransferDetailDto> TransferDetailDtos) : ICommand;