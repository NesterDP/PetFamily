using CSharpFunctionalExtensions;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.Models;
using PetFamily.SharedKernel.Structs;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;
using PetFamily.Volunteers.Contracts.Messaging;
using PetFamily.Volunteers.Domain.Entities;
using PetFamily.Volunteers.Domain.Events;
using PetFamily.Volunteers.Domain.ValueObjects.PetVO;

namespace PetFamily.Volunteers.Application.Commands.AddPet;

public class AddPetHandler : ICommandHandler<Guid, AddPetCommand>
{
    private readonly IValidator<AddPetCommand> _validator;
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly ILogger<AddPetHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRequestClient<BreedToSpeciesExistenceEvent> _client;
    private readonly IPublisher _publisher;

    public AddPetHandler(
        IValidator<AddPetCommand> validator,
        IVolunteersRepository volunteersRepository,
        ILogger<AddPetHandler> logger,
        [FromKeyedServices(UnitOfWorkSelector.Volunteers)]
        IUnitOfWork unitOfWork,
        IRequestClient<BreedToSpeciesExistenceEvent> client,
        IPublisher publisher)
    {
        _validator = validator;
        _volunteersRepository = volunteersRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _client = client;
        _publisher = publisher;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        AddPetCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToErrorList();

        var volunteerId = VolunteerId.Create(command.VolunteerId);
        var volunteerResult = await _volunteersRepository.GetByIdAsync(volunteerId, cancellationToken);
        if (volunteerResult.IsFailure)
            return volunteerResult.Error.ToErrorList();

        var petClassification = await GetPetClassification(
            command.PetClassificationDto.SpeciesId,
            command.PetClassificationDto.BreedId,
            cancellationToken);

        if (petClassification.IsFailure)
            return petClassification.Error.ToErrorList();

        var pet = CreatePet(command, petClassification.Value);

        volunteerResult.Value.AddPet(pet);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _publisher.Publish(new PetWasChangedEvent(), cancellationToken);

        _logger.LogInformation(
            "Successfully added pet with ID = {ID} to volunteer with ID = {ID}", pet.Id.Value, volunteerId.Value);

        return pet.Id.Value;
    }

    private Pet CreatePet(AddPetCommand command, PetClassification petClassification)
    {
        var petId = PetId.NewPetId();

        var name = Name.Create(command.Name).Value;

        var description = Description.Create(command.Description).Value;

        var color = Color.Create(command.Color).Value;

        var healthInfo = HealthInfo.Create(command.HealthInfo).Value;

        var address = Address.Create(
            command.AddressDto.City,
            command.AddressDto.House,
            command.AddressDto.Apartment).Value;

        var weight = Weight.Create(command.Weight).Value;

        var height = Height.Create(command.Height).Value;

        var ownerPhoneNUmber = Phone.Create(command.OwnerPhoneNumber).Value;

        var isCastrated = IsCastrated.Create(command.IsCastrated).Value;

        var dateOfBirth = DateOfBirth.Create(command.DateOfBirth.ToUniversalTime()).Value;

        var isVaccinated = IsVaccinated.Create(command.IsVaccinated).Value;

        Enum.TryParse(command.HelpStatus, out PetStatus status);
        var helpStatus = HelpStatus.Create(status).Value;

        List<TransferDetail> transferDetailsList = [];
        foreach (var transferDetail in command.TransferDetailsDto)
        {
            var result = TransferDetail.Create(transferDetail.Name, transferDetail.Description);
            transferDetailsList.Add(result.Value);
        }

        // ReSharper disable once CollectionNeverUpdated.Local
        var photosListResult = new List<Photo>();

        var pet = new Pet(
            petId,
            name,
            description,
            petClassification,
            color,
            healthInfo,
            address,
            weight,
            height,
            ownerPhoneNUmber,
            isCastrated,
            dateOfBirth,
            isVaccinated,
            helpStatus,
            transferDetailsList,
            photosListResult);

        return pet;
    }

    private async Task<Result<PetClassification, Error>> GetPetClassification(
        Guid clientSpeciesId,
        Guid clientBreedId,
        CancellationToken cancellationToken)
    {
        var existenceEvent = new BreedToSpeciesExistenceEvent(clientSpeciesId, clientBreedId);
        var checkResult = await _client.GetResponse<ResponseWrapper>(existenceEvent, cancellationToken);
        if (checkResult.Message.Text != DomainConstants.OK)
            return Errors.General.Conflict(checkResult.Message.Text);

        return PetClassification.Create(clientSpeciesId, clientBreedId).Value;
    }
}