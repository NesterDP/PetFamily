using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Abstractions;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Species.Application.SpeciesManagement.Commands.DeleteSpeciesById;

public class DeleteSpeciesByIdHandler : ICommandHandler<Guid, DeleteSpeciesByIdCommand>
{
    private readonly ISpeciesRepository _speciesRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteSpeciesByIdHandler> _logger;
    private readonly IReadDbContext _readDbContext;

    public DeleteSpeciesByIdHandler(
        ISpeciesRepository speciesRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteSpeciesByIdHandler> logger,
        IReadDbContext readDbContext)
    {
        _speciesRepository = speciesRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _readDbContext = readDbContext;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        DeleteSpeciesByIdCommand command,
        CancellationToken cancellationToken)
    {
        /*var isUsed = await _readDbContext.Pets
            .FirstOrDefaultAsync(p => p.SpeciesId == command.Id, cancellationToken);
        
        if (isUsed != null)
            return Errors.General
                .Conflict($"pets with SpeciesId = {command.Id} are still in database")
                .ToErrorList();*/

        var speciesResult = await _speciesRepository.GetByIdAsync(command.Id, cancellationToken);
        if (speciesResult.IsFailure)
            return Errors.General.ValueNotFound().ToErrorList();

        _speciesRepository.Delete(speciesResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully deleted species with ID = {ID}", command.Id);

        return command.Id;
    }
}