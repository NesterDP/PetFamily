using System.ComponentModel.DataAnnotations;
using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Extensions;
using PetFamily.Domain.PetContext.Entities;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Application.Volunteers.Create;

public class CreateVolunteerHandler
{
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly ILogger<CreateVolunteerHandler> _logger;
    private readonly IValidator<CreateVolunteerCommand> _validator;

    public CreateVolunteerHandler(
        IVolunteersRepository volunteersRepository,
        ILogger<CreateVolunteerHandler> logger,
        IValidator<CreateVolunteerCommand> validator)
    {
        _volunteersRepository = volunteersRepository;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        CreateVolunteerCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToErrorList();
        

        var volunteerId = VolunteerId.NewVolunteerId();

        var fullNameCreateResult = FullName.Create(
            command.VolunteerDto.FullName.FirstName,
            command.VolunteerDto.FullName.LastName,
            command.VolunteerDto.FullName.Surname);

        var emailCreateResult = Email.Create(command.VolunteerDto.Email);

        var descriptionCreateResult = Description.Create(command.VolunteerDto.Description);

        var experienceCreateResult = Experience.Create(command.VolunteerDto.Experience);

        var phoneNumberCreateResult = Phone.Create(command.VolunteerDto.PhoneNumber);

        List<SocialNetwork> socialNetworksList = [];
        foreach (var socialNetwork in command.SocialNetworksDto)
        {
            var result = SocialNetwork.Create(socialNetwork.Name, socialNetwork.Link);
            socialNetworksList.Add(result.Value);
        }

        var socialNetworksListCreateResult = SocialNetworksList.Create(socialNetworksList);

        List<TransferDetail> transferDetailsList = [];
        foreach (var transferDetail in command.TransferDetailsDto)
        {
            var result = TransferDetail.Create(transferDetail.Name, transferDetail.Description);
            transferDetailsList.Add(result.Value);
        }

        var transferDetailsListCreateResult = TransferDetailsList.Create(transferDetailsList);

        var volunteer = Volunteer.Create(
            volunteerId,
            fullNameCreateResult.Value,
            emailCreateResult.Value,
            descriptionCreateResult.Value,
            experienceCreateResult.Value,
            phoneNumberCreateResult.Value,
            socialNetworksListCreateResult.Value,
            transferDetailsListCreateResult.Value);

        await _volunteersRepository.AddAsync(volunteer.Value, cancellationToken);

        _logger.LogInformation(
            "Created volunteer with ID = {id}", volunteerId.Value);
        
        return volunteer.Value.Id.Value;
    }
}