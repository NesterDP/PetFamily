using CSharpFunctionalExtensions;
using PetFamily.Application.FilesManagement.Delete;
using PetFamily.Application.FilesManagement.Upload;
using PetFamily.Application.Providers;
using PetFamily.Domain.Shared.CustomErrors;

namespace PetFamily.Application.TestControllers.DeleteFIle;

public class DeleteFileHandler
{
    private readonly IFilesProvider _filesProvider;

    public DeleteFileHandler(IFilesProvider filesProvider)
    {
        _filesProvider = filesProvider;
    }

    public async Task<Result<string, Error>> HandleAsync(
        DeleteFileRequest deleteFileRequest,
        CancellationToken cancellationToken = default)
    {
        return await _filesProvider.DeleteFile(deleteFileRequest.DeleteFileData, cancellationToken);
    }
}