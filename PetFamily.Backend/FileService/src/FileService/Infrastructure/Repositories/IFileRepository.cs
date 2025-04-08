using CSharpFunctionalExtensions;
using FileService.Core;
using FileService.Core.CustomErrors;

namespace FileService.Infrastructure.Repositories;

public interface IFileRepository
{
    Task<Result<Guid, Error>> Add(FileData fileData, CancellationToken cancellationToken);
    Task<Result<IReadOnlyCollection<FileData>, Error>> Get(
        IEnumerable<Guid> fileIds,
        CancellationToken cancellationToken);
    Task<UnitResult<Error>> DeleteMany(IEnumerable<Guid> fileIds, CancellationToken cancellationToken);
}