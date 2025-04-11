using CSharpFunctionalExtensions;
using FileService.Core;
using FileService.Core.CustomErrors;
using FileService.Core.Models;
using FileService.Infrastructure.MongoDataAccess;
using MongoDB.Driver;

namespace FileService.Infrastructure.Repositories;

public class FilesRepository : IFilesRepository
{
    private readonly FileMongoDbContext _dbContext;

    public FilesRepository(FileMongoDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Result<Guid, Error>> Add(FileData fileData, CancellationToken cancellationToken)
    {
        await _dbContext.Files.InsertOneAsync(fileData, cancellationToken: cancellationToken);
        return fileData.Id;
    }

    public async Task<Result<IReadOnlyCollection<FileData>, Error>> Get(
        IEnumerable<Guid> fileIds,
        CancellationToken cancellationToken)
    {
        var filesData = await _dbContext.Files
            .Find(f => fileIds.Contains(f.Id)).ToListAsync(cancellationToken);

        if (filesData.Count != fileIds.Count())
            return Errors.General.ValueNotFound("Some of the provided IDs do not exist in the database", true);

        return filesData;
    }

    public async Task<UnitResult<Error>> DeleteMany(IEnumerable<Guid> fileIds, CancellationToken cancellationToken)
    {
        var existedCount = await _dbContext.Files.CountDocumentsAsync(f =>
            fileIds.Contains(f.Id), cancellationToken: cancellationToken);
        
        var deleteResult = await _dbContext.Files
            .DeleteManyAsync(f => fileIds.Contains(f.Id), cancellationToken: cancellationToken);

        if (deleteResult.DeletedCount != existedCount)
            return Errors.General.Failure("failed to delete files from MongoDB");

        return Result.Success<Error>();
    }
    
    public async Task<Result<FileData, Error>> GetByKey(
        string key,
        CancellationToken cancellationToken)
    {
        var filesData = await _dbContext.Files
            .Find(f => f.StoragePath == key).FirstOrDefaultAsync(cancellationToken);

        if (filesData is null)
            return Errors.General.ValueNotFound("fileData with such storagePath doesn't exist in database", true);

        return filesData;
    }
}