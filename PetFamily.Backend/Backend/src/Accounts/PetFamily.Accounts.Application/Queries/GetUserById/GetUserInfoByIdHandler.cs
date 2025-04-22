using CSharpFunctionalExtensions;
using FileService.Communication;
using FileService.Contracts.Requests;
using PetFamily.Accounts.Application.Abstractions;
using PetFamily.Accounts.Contracts.Dto;
using PetFamily.Core.Abstractions;
using PetFamily.SharedKernel.CustomErrors;
using FullNameDto = PetFamily.Accounts.Contracts.Dto.FullNameDto;
using SocialNetworkDto = PetFamily.Accounts.Contracts.Dto.SocialNetworkDto;

namespace PetFamily.Accounts.Application.Queries.GetUserById;

public class GetUserInfoByIdHandler : IQueryHandler<Result<UserInfoDto, ErrorList>, GetUserInfoByIdQuery>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IFileService _fileService;

    public GetUserInfoByIdHandler(IAccountRepository accountRepository, IFileService fileService)
    {
        _accountRepository = accountRepository;
        _fileService = fileService;
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
            var request = new GetFilesPresignedUrlsRequest([result.Value.Avatar.Id]);
            var avatarData = await _fileService.GetFilesPresignedUrls(request, cancellationToken);
            if (avatarData.IsFailure)
                return Errors.General.Failure(avatarData.Error).ToErrorList();

            avatar.Id = result.Value.Avatar.Id;
            avatar.Url = avatarData.Value.FilesInfos.FirstOrDefault(d => d.Id == avatar.Id)!.Url;
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