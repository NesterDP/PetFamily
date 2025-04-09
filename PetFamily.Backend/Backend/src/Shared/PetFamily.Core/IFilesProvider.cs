using CSharpFunctionalExtensions;
using PetFamily.Core.Files.FilesData;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.ValueObjects;
using FileInfo = PetFamily.Core.Files.FilesData.FileInfo;

namespace PetFamily.Core;

public interface IFilesProvider
{
    public Task<Result<IReadOnlyList<FilePath>, Error>> UploadFiles(
        IEnumerable<FileData> filesData,
        CancellationToken cancellationToken = default);

    public Task<Result<IReadOnlyList<FilePath>, Error>> DeleteFiles(
        IEnumerable<FileInfo> deleteData,
        CancellationToken cancellationToken = default);
    
}