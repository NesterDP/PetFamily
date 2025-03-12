using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Core;
using PetFamily.Core.Abstractions;
using PetFamily.Core.CustomErrors;
using PetFamily.Core.Extensions;
using PetFamily.Core.Files.FilesData;
using PetFamily.Core.Messaging;
using PetFamily.Core.SharedVO;
using PetFamily.Volunteers.Domain.ValueObjects.PetVO;
using PetFamily.Volunteers.Domain.ValueObjects.VolunteerVO;
using FileInfo = PetFamily.Core.Files.FilesData.FileInfo;

namespace PetFamily.Volunteers.Application.VolunteersManagement.Commands.UploadPhotosToPet;

public class UploadPhotosToPetHandler : ICommandHandler<Guid, UploadPhotosToPetCommand>
{
    private const string BUCKET_NAME = "photos";
    private readonly IValidator<UploadPhotosToPetCommand> _validator;
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly ILogger<UploadPhotosToPetHandler> _logger;
    private readonly IFilesProvider _filesProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageQueue<IEnumerable<FileInfo>> _messageQueue;

    public UploadPhotosToPetHandler(
        IValidator<UploadPhotosToPetCommand> validator,
        IVolunteersRepository volunteersRepository,
        ILogger<UploadPhotosToPetHandler> logger,
        IFilesProvider filesProvider,
        IUnitOfWork unitOfWork,
        IMessageQueue<IEnumerable<FileInfo>> messageQueue)
    {
        _validator = validator;
        _volunteersRepository = volunteersRepository;
        _logger = logger;
        _filesProvider = filesProvider;
        _unitOfWork = unitOfWork;
        _messageQueue = messageQueue;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        UploadPhotosToPetCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToErrorList();


        var volunteerId = VolunteerId.Create(command.VolunteerId);
        var volunteerResult = await _volunteersRepository.GetByIdAsync(volunteerId, cancellationToken);
        if (volunteerResult.IsFailure)
            return volunteerResult.Error.ToErrorList();

        var petId = PetId.Create(command.PetId);
        var pet = volunteerResult.Value.GetPetById(petId);
        if (pet.IsFailure)
            return pet.Error.ToErrorList();

        // работа с файлами
        List<FileData> filesData = [];
        foreach (var file in command.Files)
        {
            var extension = Path.GetExtension(file.FileName);

            var filePath = FilePath.Create(Guid.NewGuid(), extension);
            if (filePath.IsFailure)
                return filePath.Error.ToErrorList();

            var fileData = new FileData(file.Content, new FileInfo(filePath.Value, BUCKET_NAME));

            filesData.Add(fileData);
        }

        var filePathsResult = await _filesProvider.UploadFiles(filesData, cancellationToken);
        if (filePathsResult.IsFailure)
        {
            await _messageQueue.WriteAsync(filesData.Select(f => f.Info), cancellationToken);
            return filePathsResult.Error.ToErrorList();
        }

        var photoFiles = filePathsResult.Value
            .Select(f => new Photo(f))
            .ToList();

        // новый список фото должен содержать как добавленные, так и старые фото
        var unionOfPhotos = pet.Value.PhotosList.Union(photoFiles);
        volunteerResult.Value.UpdatePetPhotos(pet.Value.Id, unionOfPhotos);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Successfully uploaded photos to pet with ID = {ID}", petId.Value);

        return petId.Value;
    }
}