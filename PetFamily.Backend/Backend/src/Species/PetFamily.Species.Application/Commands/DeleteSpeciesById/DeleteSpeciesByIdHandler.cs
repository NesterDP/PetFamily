using CSharpFunctionalExtensions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Abstractions;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.Models;
using PetFamily.SharedKernel.Structs;
using PetFamily.Species.Contracts.Messaging;

namespace PetFamily.Species.Application.Commands.DeleteSpeciesById;

public class DeleteSpeciesByIdHandler : ICommandHandler<Guid, DeleteSpeciesByIdCommand>
{
    private readonly ISpeciesRepository _speciesRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteSpeciesByIdHandler> _logger;
    private readonly IRequestClient<SpeciesToPetExistenceEvent> _client;

    public DeleteSpeciesByIdHandler(
        ISpeciesRepository speciesRepository,
        [FromKeyedServices(UnitOfWorkSelector.Species)]
        IUnitOfWork unitOfWork,
        ILogger<DeleteSpeciesByIdHandler> logger,
        IRequestClient<SpeciesToPetExistenceEvent> client)
    {
        _speciesRepository = speciesRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _client = client;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        DeleteSpeciesByIdCommand command,
        CancellationToken cancellationToken)
    {
        var existenceEvent = new SpeciesToPetExistenceEvent(command.Id);
        var checkResult = await _client.GetResponse<ResponseWrapper>(existenceEvent, cancellationToken);
        if (checkResult.Message.Text != DomainConstants.OK)
            return Errors.General.Conflict(checkResult.Message.Text).ToErrorList();

        var speciesResult = await _speciesRepository.GetByIdAsync(command.Id, cancellationToken);
        if (speciesResult.IsFailure)
            return Errors.General.ValueNotFound().ToErrorList();

        _speciesRepository.Delete(speciesResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully deleted species with ID = {ID}", command.Id);

        return command.Id;
    }
}