using FileService.Infrastructure.Providers;
using FileService.Infrastructure.Repositories;
using Hangfire;

namespace FileService.Jobs;

public class ConfirmConsistencyJob
{
    private readonly IFileRepository _fileRepository;
    private readonly IFileProvider _fileProvider;
    private readonly ILogger<ConfirmConsistencyJob> _logger;

    public ConfirmConsistencyJob(
        IFileRepository fileRepository,
        IFileProvider fileProvider,
        ILogger<ConfirmConsistencyJob> logger)
    {
        _fileRepository = fileRepository;
        _fileProvider = fileProvider;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 4, DelaysInSeconds = [60, 3600, 21600, 57600])]
    public async Task Execute(
        Guid fileId,
        string key,
        string deleteJobId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start ConfirmConsistencyJob with FileId = {fileId} and Key = {key}", fileId, key);

        await _fileProvider.ConfirmExistence(key);

        var result = await _fileRepository.Get([fileId], cancellationToken);
        if (result.IsFailure)
            throw new Exception(result.Error.Message);

        BackgroundJob.Delete(deleteJobId);
        
        _logger.LogInformation("End ConfirmConsistencyJob");
    }
}