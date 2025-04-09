using CSharpFunctionalExtensions;
using PetFamily.Accounts.Application.Abstractions;
using PetFamily.Accounts.Application.Dto;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Shared;
using PetFamily.Core.Dto.Volunteer;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Accounts.Application.Queries.GetUserById;

public class GetUserInfoByIdHandler : IQueryHandler<Result<UserInfoDto, ErrorList>, GetUserInfoByIdQuery>
{
    private readonly IAccountRepository _accountRepository;

    public GetUserInfoByIdHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<Result<UserInfoDto, ErrorList>> HandleAsync(
        GetUserInfoByIdQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _accountRepository.GetUserById(query.UserId);

        if (result.IsFailure)
            return result.Error.ToErrorList();

        ParticipantAccountDto? participantAccountDto = null;
        VolunteerAccountDto? volunteerAccountDto = null;
        AdminAccountDto? adminAccountDto = null;


        if (result.Value.ParticipantAccount != null)
        {
            participantAccountDto = new ParticipantAccountDto();
            participantAccountDto.FavoritePets = result.Value.ParticipantAccount!.FavoritePets;
        }

        if (result.Value.VolunteerAccount != null)
        {
            volunteerAccountDto = new VolunteerAccountDto();
            volunteerAccountDto.Experience = result.Value.VolunteerAccount.Experience;
            volunteerAccountDto.TransferDetails = result.Value.VolunteerAccount.TransferDetails
                .Select(t => new TransferDetailDto(t.Name, t.Description)).ToList();
            volunteerAccountDto.Certificates = result.Value.VolunteerAccount.Certificates;
        }
        
        if (result.Value.AdminAccount != null)
            adminAccountDto = new AdminAccountDto();
        
        
        var avatar = new AvatarDto();
        if (result.Value.Avatar.Id is not null)
        {
            // TODO: здесь должен быть запрос к FileService на получение URL по Id файла
        }

        var userInfoDto = new UserInfoDto()
        {
            FullName = new FullNameDto(
                result.Value.FullName.FirstName,
                result.Value.FullName.LastName,
                result.Value.FullName.Surname),
            Avatar = avatar,
            Email = result.Value.Email,
            PhoneNumber = result.Value.PhoneNumber,
            Roles = result.Value.Roles.Select(r => new RoleDto(r.Name)).ToList(),
            SocialNetworks = result.Value.SocialNetworks
                .Select(s => new SocialNetworkDto(s.Name, s.Link)).ToList(),
            ParticipantAccount = participantAccountDto,
            VolunteerAccount = volunteerAccountDto,
            AdminAccount = adminAccountDto
        };

        return userInfoDto;
    }
}