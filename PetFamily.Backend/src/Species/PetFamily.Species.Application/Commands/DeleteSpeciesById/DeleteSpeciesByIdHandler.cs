using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Abstractions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.Structs;
using PetFamily.Volunteers.Contracts;
using PetFamily.Volunteers.Contracts.Requests;

namespace PetFamily.Species.Application.Commands.DeleteSpeciesById;

public class DeleteSpeciesByIdHandler : ICommandHandler<Guid, DeleteSpeciesByIdCommand>
{
    private readonly ISpeciesRepository _speciesRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteSpeciesByIdHandler> _logger;
    private readonly ISpeciesToPetExistenceContract _contract;

    public DeleteSpeciesByIdHandler(
        ISpeciesRepository speciesRepository,
        [FromKeyedServices(UnitOfWorkSelector.Species)] IUnitOfWork unitOfWork,
        ILogger<DeleteSpeciesByIdHandler> logger,
        ISpeciesToPetExistenceContract contract)
    {
        _speciesRepository = speciesRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _contract = contract;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        DeleteSpeciesByIdCommand command,
        CancellationToken cancellationToken)
    {
        var request = new SpeciesToPetExistenceRequest(command.Id);
        var checkResult = await _contract.SpeciesToPetExistence(request, cancellationToken);
        if (checkResult.IsFailure)
            return checkResult.Error.ToErrorList();

        var speciesResult = await _speciesRepository.GetByIdAsync(command.Id, cancellationToken);
        if (speciesResult.IsFailure)
            return Errors.General.ValueNotFound().ToErrorList();

        _speciesRepository.Delete(speciesResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully deleted species with ID = {ID}", command.Id);

        return command.Id;
    }
}