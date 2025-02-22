using CSharpFunctionalExtensions;
using PetFamily.Application.Providers;
using PetFamily.Domain.Shared.CustomErrors;

namespace PetFamily.Application.FilesManagement.Delete;

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