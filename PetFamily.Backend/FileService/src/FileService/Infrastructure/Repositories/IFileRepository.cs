using CSharpFunctionalExtensions;
using FileService.Core.CustomErrors;
using FileService.Core.Models;

namespace FileService.Infrastructure.Repositories;

public interface IFileRepository
{
    Task<Result<Guid, Error>> Add(FileData fileData, CancellationToken cancellationToken);
    Task<Result<IReadOnlyCollection<FileData>, Error>> Get(
        IEnumerable<Guid> fileIds,
        CancellationToken cancellationToken);
    Task<UnitResult<Error>> DeleteMany(IEnumerable<Guid> fileIds, CancellationToken cancellationToken);
}