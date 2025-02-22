using CSharpFunctionalExtensions;
using PetFamily.Application.Providers;
using PetFamily.Domain.Shared.CustomErrors;

namespace PetFamily.Application.FilesManagement.Upload;

public class UploadFileHandler
{
    private readonly IFilesProvider _filesProvider;

    public UploadFileHandler(IFilesProvider filesProvider)
    {
        _filesProvider = filesProvider;
    }

    public async Task<Result<string, Error>> HandleAsync(
        UploadFileRequest uploadFileRequest,
        CancellationToken cancellationToken = default)
    {
        return await _filesProvider.UploadFile(uploadFileRequest.UploadData, cancellationToken);
    }
}