using CSharpFunctionalExtensions;
using FileService.Communication;
using FileService.Contracts.Requests;
using FileService.Contracts.Responses;
using FileService.Contracts.SubModels;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Accounts.Application.Abstractions;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.Structs;
using PetFamily.SharedKernel.ValueObjects;

namespace PetFamily.Accounts.Application.Commands.CompleteUploadAvatar;

public class CompleteUploadAvatarHandler
    : ICommandHandler<CompleteMultipartUploadResponse, CompleteUploadAvatarCommand>
{
    private readonly IValidator<CompleteUploadAvatarCommand> _validator;
    private readonly IAccountRepository _accountRepository;
    private readonly ILogger<CompleteUploadAvatarHandler> _logger;
    private readonly IFileService _fileService;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteUploadAvatarHandler(
        IValidator<CompleteUploadAvatarCommand> validator,
        IAccountRepository accountRepository,
        ILogger<CompleteUploadAvatarHandler> logger,
        IFileService fileService,
        [FromKeyedServices(UnitOfWorkSelector.Accounts)]
        IUnitOfWork unitOfWork)
    {
        _validator = validator;
        _accountRepository = accountRepository;
        _logger = logger;
        _fileService = fileService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CompleteMultipartUploadResponse, ErrorList>> HandleAsync(
        CompleteUploadAvatarCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToErrorList();

        var userResult = await _accountRepository.GetUserById(command.UserId);
        if (userResult.IsFailure)
            return userResult.Error.ToErrorList();

        // межсерверное взаимодействие, удаление предыдущей фотографии пользователя
        if (userResult.Value.Avatar.Id is not null)
        {
            var deleteRequest = new DeleteFilesByIdsRequest([userResult.Value.Avatar.Id!.Value]);
            var deletionResult = await _fileService.DeleteFilesByIds(deleteRequest, cancellationToken);
            if (deletionResult.IsFailure)
                return Errors.General.Failure(deletionResult.Error).ToErrorList();
        }

        // межсерверное взаимодействие, подтверждение загрузки новой фото
        var clientInfo = new MultipartCompleteClientInfo(
            command.FileInfo.Key,
            command.FileInfo.UploadId,
            command.FileInfo.Parts.Select(p => new PartETagInfo(p.PartNumber, p.ETag)).ToList());

        var uploadRequest = new CompleteMultipartUploadRequest([clientInfo]);

        var uploadResult = await _fileService.CompleteMultipartUpload(uploadRequest, cancellationToken);
        if (uploadResult.IsFailure)
            return Errors.General.Failure(uploadResult.Error).ToErrorList();

        // сохранение информации о новой фото в БД модуля
        var newAvatar = Avatar.Create(
            uploadResult.Value.MultipartCompleteInfos.FirstOrDefault()!.FileId,
            uploadResult.Value.MultipartCompleteInfos.FirstOrDefault()!.ContentType);

        if (newAvatar.IsFailure)
            return Errors.General.ValueIsInvalid("contentType").ToErrorList();

        userResult.Value.Avatar = newAvatar.Value;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Successfully changed photo of user with ID = {ID}", userResult.Value.Id);

        return uploadResult.Value;
    }
}