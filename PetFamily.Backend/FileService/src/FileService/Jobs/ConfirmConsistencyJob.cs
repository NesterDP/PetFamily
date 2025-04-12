using FileService.Infrastructure.Providers;
using FileService.Infrastructure.Repositories;
using Hangfire;

namespace FileService.Jobs;

public class ConfirmConsistencyJob
{
    private readonly IFilesRepository _filesRepository;
    private readonly IFilesProvider _filesProvider;
    private readonly ILogger<ConfirmConsistencyJob> _logger;

    public ConfirmConsistencyJob(
        IFilesRepository filesRepository,
        IFilesProvider filesProvider,
        ILogger<ConfirmConsistencyJob> logger)
    {
        _filesRepository = filesRepository;
        _filesProvider = filesProvider;
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

        await _filesProvider.ConfirmExistence(key);

        var result = await _filesRepository.Get([fileId], cancellationToken);
        if (result.IsFailure)
            throw new Exception(result.Error.Message);

        BackgroundJob.Delete(deleteJobId);
        
        _logger.LogInformation("End ConfirmConsistencyJob");
    }
}