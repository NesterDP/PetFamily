using CSharpFunctionalExtensions;
using PetFamily.Application.Files.FilesData;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.Shared.SharedVO;
using FileInfo = PetFamily.Application.Files.FilesData.FileInfo;

namespace PetFamily.Application.Files;

public interface IFilesProvider
{
    public Task<Result<IReadOnlyList<FilePath>, Error>> UploadFiles(
        IEnumerable<FileData> filesData,
        CancellationToken cancellationToken = default);

    public Task<Result<IReadOnlyList<FilePath>, Error>> DeleteFiles(
        IEnumerable<FileInfo> deleteData,
        CancellationToken cancellationToken = default);
    
}