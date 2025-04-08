using CSharpFunctionalExtensions;
using FileService.Core;
using FileService.Core.CustomErrors;
using FileService.Infrastructure.MongoDataAccess;
using MongoDB.Driver;

namespace FileService.Infrastructure.Repositories;

public class FileRepository : IFileRepository
{
    private readonly FileMongoDbContext _dbContext;

    public FileRepository(FileMongoDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Result<Guid, Error>> Add(FileData fileData, CancellationToken cancellationToken)
    {
        await _dbContext.Files.InsertOneAsync(fileData, cancellationToken: cancellationToken);
        return fileData.Id;
    }

    public async Task<IReadOnlyCollection<FileData>> Get(IEnumerable<Guid> fileIds, CancellationToken cancellationToken)
    {
        return await _dbContext.Files.Find(f => fileIds.Contains(f.Id)).ToListAsync(cancellationToken);
    }

    public async Task<UnitResult<Error>> DeleteMany(IEnumerable<Guid> fileIds, CancellationToken cancellationToken)
    {
        var deleteResult = await _dbContext.Files
            .DeleteManyAsync(f => fileIds.Contains(f.Id), cancellationToken: cancellationToken);

        if (deleteResult.DeletedCount != fileIds.Count())
            return Errors.General.Failure("failed to delete files from MongoDB");

        return Result.Success<Error>();

    }
}