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

namespace PetFamily.Species.Application.Commands.DeleteBreedById;

public class DeleteBreedByIdHandler : ICommandHandler<Guid, DeleteBreedByIdCommand>
{
    private readonly ISpeciesRepository _speciesRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteBreedByIdHandler> _logger;
    private readonly IRequestClient<BreedToPetExistenceEvent> _client;

    public DeleteBreedByIdHandler(
        ISpeciesRepository speciesRepository,
        [FromKeyedServices(UnitOfWorkSelector.Species)]
        IUnitOfWork unitOfWork,
        ILogger<DeleteBreedByIdHandler> logger,
        IRequestClient<BreedToPetExistenceEvent> client)
    {
        _speciesRepository = speciesRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _client = client;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        DeleteBreedByIdCommand command,
        CancellationToken cancellationToken)
    {
        var existenceEvent = new BreedToPetExistenceEvent(command.BreedId);
        var checkResult = await _client.GetResponse<ResponseWrapper>(existenceEvent, cancellationToken);
        if (checkResult.Message.Text != DomainConstants.OK)
            return Errors.General.Conflict(checkResult.Message.Text).ToErrorList();

        var speciesResult = await _speciesRepository.GetByIdAsync(command.SpeciesId, cancellationToken);
        if (speciesResult.IsFailure)
            return Errors.General.ValueNotFound().ToErrorList();

        speciesResult.Value.RemoveBreedById(command.BreedId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully deleted breed with ID = {ID}", command.BreedId);

        return command.BreedId;
    }
}