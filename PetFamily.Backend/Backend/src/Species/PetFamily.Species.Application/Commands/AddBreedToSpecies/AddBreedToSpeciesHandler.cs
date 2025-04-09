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
using PetFamily.Species.Domain.Entities;

namespace PetFamily.Species.Application.Commands.AddBreedToSpecies;

public class AddBreedToSpeciesHandler : ICommandHandler<Guid, AddBreedToSpeciesCommand>
{
    private readonly IValidator<AddBreedToSpeciesCommand> _validator;
    private readonly ISpeciesRepository _speciesRepository;
    private readonly ILogger<AddBreedToSpeciesHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public AddBreedToSpeciesHandler(
        IValidator<AddBreedToSpeciesCommand> validator,
        ISpeciesRepository speciesRepository,
        ILogger<AddBreedToSpeciesHandler> logger,
        [FromKeyedServices(UnitOfWorkSelector.Species)] IUnitOfWork unitOfWork)
    {
        _validator = validator;
        _speciesRepository = speciesRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        AddBreedToSpeciesCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToErrorList();


        var speciesId = SpeciesId.Create(command.Id);
        var speciesResult = await _speciesRepository.GetByIdAsync(speciesId, cancellationToken);
        if (speciesResult.IsFailure)
            return speciesResult.Error.ToErrorList();

        var id = BreedId.NewBreedId();

        var name = Name.Create(command.Name).Value;

        var breed = new Breed(id, name);

        speciesResult.Value.AddBreed(breed);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Successfully added breed with ID = {ID} to species with ID = {ID}", breed.Id.Value, speciesId.Value);

        return breed.Id.Value;
    }
}