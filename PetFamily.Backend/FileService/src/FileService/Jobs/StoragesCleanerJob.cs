using FileService.Infrastructure.Providers;
using FileService.Infrastructure.Repositories;
using Hangfire;

namespace FileService.Jobs;

public class StoragesCleanerJob
{
    private readonly IFilesRepository _filesRepository;
    private readonly IFilesProvider _filesProvider;
    private readonly ILogger<StoragesCleanerJob> _logger;

    public StoragesCleanerJob(
        IFilesRepository filesRepository,
        IFilesProvider filesProvider,
        ILogger<StoragesCleanerJob> logger)
    {
        _filesRepository = filesRepository;
        _filesProvider = filesProvider;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 3, DelaysInSeconds = [86400, 86400, 86400])]
    public async Task Execute(
        Guid fileId,
        string key,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start ClearStoragesJob with fileId = {fileId} and Key = {key}", fileId, key);

         await _filesRepository.DeleteMany([fileId], cancellationToken);
         
         await _filesProvider.DeleteFiles([key], cancellationToken);
        
        _logger.LogInformation("End ClearStoragesJob");
    }
}