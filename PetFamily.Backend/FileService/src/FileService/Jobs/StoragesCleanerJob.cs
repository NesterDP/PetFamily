using FileService.Infrastructure.Providers;
using FileService.Infrastructure.Repositories;
using Hangfire;

namespace FileService.Jobs;

public class StoragesCleanerJob
{
    private readonly IFileRepository _fileRepository;
    private readonly IFileProvider _fileProvider;
    private readonly ILogger<StoragesCleanerJob> _logger;

    public StoragesCleanerJob(
        IFileRepository fileRepository,
        IFileProvider fileProvider,
        ILogger<StoragesCleanerJob> logger)
    {
        _fileRepository = fileRepository;
        _fileProvider = fileProvider;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 3, DelaysInSeconds = [86400, 86400, 86400])]
    public async Task Execute(
        Guid fileId,
        string key,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start ClearStoragesJob with fileId = {fileId} and Key = {key}", fileId, key);

         await _fileRepository.DeleteMany([fileId], cancellationToken);
         
         await _fileProvider.DeleteFiles([key], cancellationToken);
        
        _logger.LogInformation("End ClearStoragesJob");
    }
}