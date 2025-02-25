using CSharpFunctionalExtensions;
using PetFamily.Application.FilesProvider.FilesData;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Application.FilesProvider;

public interface IFilesProvider
{
    public Task<Result<IReadOnlyList<FilePath>, Error>> UploadFiles(
        IEnumerable<FileData> filesData,
        CancellationToken cancellationToken = default);

    public Task<Result<IReadOnlyList<string>, Error>> DeleteFiles(
        IEnumerable<DeleteData> deleteData,
        CancellationToken cancellationToken = default);
    
}