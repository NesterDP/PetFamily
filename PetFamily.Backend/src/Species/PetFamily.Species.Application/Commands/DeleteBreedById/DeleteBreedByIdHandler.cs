using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Abstractions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.Structs;
using PetFamily.Volunteers.Contracts;
using PetFamily.Volunteers.Contracts.Requests;

namespace PetFamily.Species.Application.Commands.DeleteBreedById;

public class DeleteBreedByIdHandler : ICommandHandler<Guid, DeleteBreedByIdCommand>
{
    private readonly ISpeciesRepository _speciesRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteBreedByIdHandler> _logger;
    private readonly IBreedToPetExistenceContract _contract;
    
    public DeleteBreedByIdHandler(
        ISpeciesRepository speciesRepository,
        [FromKeyedServices(UnitOfWorkSelector.Species)] IUnitOfWork unitOfWork,
        ILogger<DeleteBreedByIdHandler> logger,
        IBreedToPetExistenceContract contract)
    {
        _speciesRepository = speciesRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _contract = contract;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        DeleteBreedByIdCommand command,
        CancellationToken cancellationToken)
    {
        var request = new BreedToPetExistenceRequest(command.BreedId);
        var checkResult = await _contract.BreedToPetExistence(request, cancellationToken);
        if (checkResult.IsFailure)
            return checkResult.Error.ToErrorList();
        
        var speciesResult = await _speciesRepository.GetByIdAsync(command.SpeciesId, cancellationToken);
        if (speciesResult.IsFailure)
            return Errors.General.ValueNotFound().ToErrorList();
        
        speciesResult.Value.RemoveBreedById(command.BreedId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Successfully deleted breed with ID = {ID}", command.BreedId);

        return command.BreedId;
    }

}