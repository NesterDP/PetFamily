using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Database;
using PetFamily.Application.Extensions;
using PetFamily.Application.Files;
using PetFamily.Application.Files.FilesData;
using PetFamily.Application.Messaging;
using PetFamily.Domain.PetContext.ValueObjects.PetVO;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.Shared.SharedVO;
using FileInfo = PetFamily.Application.Files.FilesData.FileInfo;

namespace PetFamily.Application.Volunteers.UploadPhotosToPet;

public class UploadPhotosToPetHandler
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
        var unionOfPhotos = pet.Value.PhotosList.Photos.Union(photoFiles);
        var updatedList = unionOfPhotos.Select(u => new Photo(u.PathToStorage)).ToList();

        volunteerResult.Value.UpdatePetPhotos(pet.Value.Id, PhotosList.Create(updatedList).Value);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Successfully uploaded photos to pet with ID = {ID}", petId.Value);

        return petId.Value;
    }
}