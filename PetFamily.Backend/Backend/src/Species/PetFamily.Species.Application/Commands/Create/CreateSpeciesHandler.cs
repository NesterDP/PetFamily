using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.Structs;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace PetFamily.Species.Application.Commands.Create;

public class CreateSpeciesHandler : ICommandHandler<Guid, CreateSpeciesCommand>
{
    private readonly ISpeciesRepository _speciesRepository;
    private readonly ILogger<CreateSpeciesHandler> _logger;
    private readonly IValidator<CreateSpeciesCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSpeciesHandler(
        ISpeciesRepository speciesRepository,
        ILogger<CreateSpeciesHandler> logger,
        IValidator<CreateSpeciesCommand> validator,
        [FromKeyedServices(UnitOfWorkSelector.Species)]
        IUnitOfWork unitOfWork)
    {
        _speciesRepository = speciesRepository;
        _logger = logger;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        CreateSpeciesCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToErrorList();

        var id = SpeciesId.NewSpeciesId();

        var name = Name.Create(command.Name).Value;

        var species = new Domain.Entities.Species(id, name);

        await _speciesRepository.AddAsync(species, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created species with ID = {id}", species.Id);

        return species.Id.Value;
    }
}