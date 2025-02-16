using System.ComponentModel.Design;
using CSharpFunctionalExtensions;
using PetFamily.Application.Dto.Shared;
using PetFamily.Application.Dto.Volunteer;
using PetFamily.Domain.PetContext.Entities;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Application.Volunteers.CreateVolunteer;

public class CreateVolunteerHandler
{
    private readonly IVolunteersRepository _volunteersRepository;

    public CreateVolunteerHandler(IVolunteersRepository volunteersRepository)
    {
        _volunteersRepository = volunteersRepository;
    }

    public async Task<Result<Guid, Error>> HandleAsync(CreateVolunteerRequest request, 
        CancellationToken cancellationToken)
    {
        var volunteerId = VolunteerId.NewVolunteerId();

        var fullNameCreateResult = FullName.Create(
            request.VolunteerDto.FirstName,
            request.VolunteerDto.LastName,
            request.VolunteerDto.Surname);
        
        if (fullNameCreateResult.IsFailure)
            return fullNameCreateResult.Error;

        var emailCreateResult = Email.Create(request.VolunteerDto.Email);
        if (emailCreateResult.IsFailure)
            return emailCreateResult.Error;
        
        var descriptionCreateResult = Description.Create(request.VolunteerDto.Description);
        if (emailCreateResult.IsFailure)
            return emailCreateResult.Error;
        
        var experienceCreateResult = Experience.Create(request.VolunteerDto.Experience);
        if (experienceCreateResult.IsFailure)
            return experienceCreateResult.Error;
        
        var phoneNumberCreateResult = Phone.Create(request.VolunteerDto.PhoneNumber);
        if (phoneNumberCreateResult.IsFailure)
            return phoneNumberCreateResult.Error;
        
        List<SocialNetwork> socialNetworkList = [];
        foreach (var socialNetwork in request.SocialNetworksDto)
        {
            var result = SocialNetwork.Create(socialNetwork.Name, socialNetwork.Link);
            if (result.IsFailure)
                return result.Error;
            
            socialNetworkList.Add(result.Value);
        }
        var socialNetworkListCreateResult = SocialNetworkList.Create(socialNetworkList);

        List<TransferDetail> transferDetailList = [];
        foreach (var transferDetail in request.TransferDetailsDto)
        {
            var result = TransferDetail.Create(transferDetail.Name, transferDetail.Description);
            if (result.IsFailure)
                return result.Error;
            
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
        
        return volunteer.Value.Id.Value;
        
    }
}