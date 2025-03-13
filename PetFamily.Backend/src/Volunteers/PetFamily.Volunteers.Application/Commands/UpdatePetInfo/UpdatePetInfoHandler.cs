using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.Structs;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.Species.Contracts;
using PetFamily.Species.Contracts.Requests;
using PetFamily.Volunteers.Domain.ValueObjects.PetVO;
using PetFamily.Volunteers.Domain.ValueObjects.VolunteerVO;

namespace PetFamily.Volunteers.Application.Commands.UpdatePetInfo;

public class UpdatePetInfoHandler : ICommandHandler<Guid, UpdatePetInfoCommand>
{
    private readonly IValidator<UpdatePetInfoCommand> _validator;
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly ILogger<UpdatePetInfoHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBreedToSpeciesExistenceContract _contract;

    public UpdatePetInfoHandler(
        IValidator<UpdatePetInfoCommand> validator,
        IVolunteersRepository volunteersRepository,
        ILogger<UpdatePetInfoHandler> logger,
        [FromKeyedServices(UnitOfWorkSelector.Volunteers)] IUnitOfWork unitOfWork,
        IBreedToSpeciesExistenceContract contract)
    {
        _validator = validator;
        _volunteersRepository = volunteersRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _contract = contract;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        UpdatePetInfoCommand command,
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

        List<TransferDetail> transferDetailsList = [];
        foreach (var transferDetail in command.TransferDetailsDto)
        {
            var result = TransferDetail.Create(transferDetail.Name, transferDetail.Description);
            transferDetailsList.Add(result.Value);
        }

        var updateResult = volunteerResult.Value.UpdatePetInfo(
            command.PetId,
            name,
            description,
            petClassification.Value,
            color,
            healthInfo,
            address,
            weight,
            height,
            ownerPhoneNUmber,
            isCastrated,
            dateOfBirth,
            isVaccinated,
            transferDetailsList);

        if (updateResult.IsFailure)
            return updateResult.Error.ToErrorList();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully updated info of pet with ID = {ID}", command.PetId);

        return command.PetId;
    }


    private async Task<Result<PetClassification, Error>> GetPetClassification(
        Guid clientSpeciesId,
        Guid clientBreedId,
        CancellationToken cancellationToken)
    {
        var request = new BreedToSpeciesExistenceRequest(clientSpeciesId, clientBreedId);
        var result = await _contract.BreedToSpeciesExistence(request, cancellationToken);
        if (result.IsFailure)
            return result.Error;

        return PetClassification.Create(clientSpeciesId, clientBreedId).Value;
    }
}