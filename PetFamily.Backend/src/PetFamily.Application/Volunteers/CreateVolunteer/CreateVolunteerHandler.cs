using System.ComponentModel.Design;
using CSharpFunctionalExtensions;
using PetFamily.Application.Dto.Shared;
using PetFamily.Application.Dto.Volunteer;
using PetFamily.Domain.PetContext.Entities;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Application.Volunteers.CreateVolunteer;

public class CreateVolunteerHandler
{
    private readonly IVolunteersRepository _volunteersRepository;

    public CreateVolunteerHandler(IVolunteersRepository volunteersRepository)
    {
        _volunteersRepository = volunteersRepository;
    }

    public async Task<Result<Guid, Error>> HandleAsync(
        VolunteerDto volunteerDto,
        List<SocialNetworkDto> socialNetworkListDto,
        List<TransferDetailDto> transferDetailListDto,
        CancellationToken cancellationToken = default)
    {
        var volunteerId = VolunteerId.NewVolunteerId();

        var fullNameCreateResult = FullName.Create(volunteerDto.FirstName, volunteerDto.LastName, volunteerDto.Surname);
        if (fullNameCreateResult.IsFailure)
            return fullNameCreateResult.Error;

        var emailCreateResult = Email.Create(volunteerDto.Email);
        if (emailCreateResult.IsFailure)
            return emailCreateResult.Error;
        
        var descriptionCreateResult = Description.Create(volunteerDto.Description);
        if (emailCreateResult.IsFailure)
            return emailCreateResult.Error;
        
        var experienceCreateResult = Experience.Create(volunteerDto.Experience);
        if (experienceCreateResult.IsFailure)
            return experienceCreateResult.Error;
        
        var phoneNumberCreateResult = Phone.Create(volunteerDto.PhoneNumber);
        if (phoneNumberCreateResult.IsFailure)
            return phoneNumberCreateResult.Error;
        
        List<SocialNetwork> socialNetworkList = [];
        foreach (var result in socialNetworkListDto.Select(sn => SocialNetwork.Create(sn.Name, sn.Link)))
        {
            if (result.IsFailure)
                return result.Error;
            
            socialNetworkList.Add(result.Value);
        }
        var socialNetworkListCreateResult = SocialNetworkList.Create(socialNetworkList);

        List<TransferDetail> transferDetailList = [];
        foreach (var result in transferDetailListDto.Select(td => TransferDetail.Create(td.Name, td.Description)))
        {
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
            transferDetailListCreateResult.Value
        );
        
        await _volunteersRepository.AddAsync(volunteer.Value, cancellationToken);
        
        return volunteer.Value.Id.Value;
        
    }
}