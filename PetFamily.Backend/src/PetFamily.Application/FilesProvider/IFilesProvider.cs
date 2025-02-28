using CSharpFunctionalExtensions;
using PetFamily.Application.FilesProvider.FilesData;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.Shared.SharedVO;
using FileInfo = PetFamily.Application.FilesProvider.FilesData.FileInfo;

namespace PetFamily.Application.FilesProvider;

public interface IFilesProvider
{
    public Task<Result<IReadOnlyList<FilePath>, Error>> UploadFiles(
        IEnumerable<FileData> filesData,
        CancellationToken cancellationToken = default);

    public Task<Result<IReadOnlyList<FilePath>, Error>> DeleteFiles(
        IEnumerable<FileInfo> deleteData,
        CancellationToken cancellationToken = default);
    
}