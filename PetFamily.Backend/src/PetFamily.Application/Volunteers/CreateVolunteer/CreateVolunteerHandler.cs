using System.ComponentModel.Design;
using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Dto.Shared;
using PetFamily.Application.Dto.Volunteer;
using PetFamily.Domain.PetContext.Entities;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.Shared.SharedVO;
using InvalidCastException = System.InvalidCastException;

namespace PetFamily.Application.Volunteers.CreateVolunteer;

public class CreateVolunteerHandler
{
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly ILogger<CreateVolunteerHandler> _logger;

    public CreateVolunteerHandler(
        IVolunteersRepository volunteersRepository,
        ILogger<CreateVolunteerHandler> logger)
    {
        _volunteersRepository = volunteersRepository;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> HandleAsync(
        CreateVolunteerRequest request, CancellationToken cancellationToken)
    {
        var volunteerId = VolunteerId.NewVolunteerId();

        var fullNameCreateResult = FullName.Create(
            request.VolunteerDto.FullName.FirstName,
            request.VolunteerDto.FullName.LastName,
            request.VolunteerDto.FullName.Surname);

        var emailCreateResult = Email.Create(request.VolunteerDto.Email);

        var descriptionCreateResult = Description.Create(request.VolunteerDto.Description);

        var experienceCreateResult = Experience.Create(request.VolunteerDto.Experience);

        var phoneNumberCreateResult = Phone.Create(request.VolunteerDto.PhoneNumber);

        List<SocialNetwork> socialNetworkList = [];
        foreach (var socialNetwork in request.SocialNetworksDto)
        {
            var result = SocialNetwork.Create(socialNetwork.Name, socialNetwork.Link);
            socialNetworkList.Add(result.Value);
        }

        var socialNetworkListCreateResult = SocialNetworkList.Create(socialNetworkList);

        List<TransferDetail> transferDetailList = [];
        foreach (var transferDetail in request.TransferDetailsDto)
        {
            var result = TransferDetail.Create(transferDetail.Name, transferDetail.Description);
            transferDetailList.Add(result.Value);
        }

        var transferDetailListCreateResult = TransferDetailList.Create(transferDetailList);

        var volunteer = Volunteer.Create(
            volunteerId,
            fullNameCreateResult.Value,
            emailCreateResult.Value,
            descriptionCreateResult.Value,
            experienceCreateResult.Value,
            phoneNumberCreateResult.Value,
            socialNetworkListCreateResult.Value,
            transferDetailListCreateResult.Value);

        await _volunteersRepository.AddAsync(volunteer.Value, cancellationToken);

        _logger.LogInformation(
            "Created volunteer with fullName = {firstName} {lastName} {surname} and ID = {id}",
            volunteer.Value.FullName.FirstName,
            volunteer.Value.FullName.LastName,
            volunteer.Value.FullName.Surname,
            volunteer.Value.Id.Value);
        
        return volunteer.Value.Id.Value;
    }
}