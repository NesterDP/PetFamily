using CSharpFunctionalExtensions;
using PetFamily.Application.FilesManagement.Upload;
using PetFamily.Application.FilesProvider.FilesData;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.Shared.SharedVO;
using FileInfo = PetFamily.Application.FilesProvider.FilesData.FileInfo;

namespace PetFamily.Application.FilesProvider.Upload;

public class UploadFilesHandler
{
    private readonly IFilesProvider _filesProvider;
    private const string BUCKET_NAME = "gigabucket";

    public UploadFilesHandler(IFilesProvider filesProvider)
    {
        _filesProvider = filesProvider;
    }

    public async Task<Result<string,ErrorList>> HandleAsync(
        UploadFileCommand command,
        CancellationToken cancellationToken = default)
    {
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
        
        var uploadResult = await _filesProvider.UploadFiles(filesData, cancellationToken);
        if (uploadResult.IsFailure)
            return Error.Failure("fail", "wasn't able to upload").ToErrorList();

        return "ok";
    }
}