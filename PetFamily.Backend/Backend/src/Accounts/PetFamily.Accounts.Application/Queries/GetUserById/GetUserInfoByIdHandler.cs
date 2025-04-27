using CSharpFunctionalExtensions;
using FileService.Communication;
using FileService.Contracts.Requests;
using Microsoft.Extensions.Caching.Distributed;
using PetFamily.Accounts.Application.Abstractions;
using PetFamily.Accounts.Contracts.Dto;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Caching;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.CustomErrors;
using FullNameDto = PetFamily.Accounts.Contracts.Dto.FullNameDto;
using SocialNetworkDto = PetFamily.Accounts.Contracts.Dto.SocialNetworkDto;

namespace PetFamily.Accounts.Application.Queries.GetUserById;

public class GetUserInfoByIdHandler : IQueryHandler<Result<UserInfoDto, ErrorList>, GetUserInfoByIdQuery>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IFileService _fileService;
    private readonly ICacheService _cache;

    private readonly DistributedCacheEntryOptions _cacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheConstants.DEFAULT_EXPIRATION_MINUTES)
    };

    public GetUserInfoByIdHandler(
        IAccountRepository accountRepository,
        IFileService fileService,
        ICacheService cache)
    {
        _accountRepository = accountRepository;
        _fileService = fileService;
        _cache = cache;
    }

    public async Task<Result<UserInfoDto, ErrorList>> HandleAsync(
        GetUserInfoByIdQuery query,
        CancellationToken cancellationToken)
    {
        string key = CacheConstants.USERS_PREFIX + query.UserId;
        var userInfo = await _cache.GetOrSetAsync(
            key,
            _cacheOptions,
            async () => await GetUserInfoDto(query),
            cancellationToken);

        if (userInfo is null)
            return Errors.General.ValueNotFound(query.UserId).ToErrorList();

        if (userInfo.Avatar.Id is not null)
        {
            var request = new GetFilesPresignedUrlsRequest([userInfo.Avatar.Id.Value]);
            var avatarData = await _fileService.GetFilesPresignedUrls(request, cancellationToken);
            if (avatarData.IsFailure)
                return Errors.General.Failure(avatarData.Error).ToErrorList();

            userInfo.Avatar.Url = avatarData.Value.FilesInfos
                .FirstOrDefault(d => d.Id == userInfo.Avatar.Id)!.Url;
        }

        return userInfo;
    }

    public async Task<UserInfoDto?> GetUserInfoDto(GetUserInfoByIdQuery query)
    {
        var user = await _accountRepository.GetNullableUserById(query.UserId);
        if (user == null)
            return null;

        ParticipantAccountDto? participantAccountDto = null;
        VolunteerAccountDto? volunteerAccountDto = null;
        AdminAccountDto? adminAccountDto = null;

        if (user.ParticipantAccount != null)
        {
            participantAccountDto = new ParticipantAccountDto();
            participantAccountDto.FavoritePets = user.ParticipantAccount!.FavoritePets;
        }

        if (user.VolunteerAccount != null)
        {
            volunteerAccountDto = new VolunteerAccountDto();
            volunteerAccountDto.Experience = user.VolunteerAccount.Experience;
            volunteerAccountDto.TransferDetails = user.VolunteerAccount.TransferDetails
                .Select(t => new TransferDetailDto(t.Name, t.Description)).ToList();
            volunteerAccountDto.Certificates = user.VolunteerAccount.Certificates;
        }

        if (user.AdminAccount != null)
            adminAccountDto = new AdminAccountDto();

        var avatar = new AvatarDto();
        if (user.Avatar.Id is not null)
            avatar.Id = user.Avatar.Id;

        var userInfoDto = new UserInfoDto()
        {
            FullName = new FullNameDto(
                user.FullName.FirstName,
                user.FullName.LastName,
                user.FullName.Surname),
            Avatar = avatar,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber!,
            Roles = user.Roles.Select(r => new RoleDto(r.Name!)).ToList(),
            SocialNetworks = user.SocialNetworks
                .Select(s => new SocialNetworkDto(s.Name, s.Link)).ToList(),
            ParticipantAccount = participantAccountDto,
            VolunteerAccount = volunteerAccountDto,
            AdminAccount = adminAccountDto,
        };

        return userInfoDto;
    }
}