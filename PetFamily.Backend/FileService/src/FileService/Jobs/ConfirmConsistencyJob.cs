using Amazon.S3;
using FileService.Infrastructure.Repositories;
using Hangfire;

namespace FileService.Jobs;

public class ConfirmConsistencyJob
{
    private readonly IFileRepository _fileRepository;
    private readonly IAmazonS3 _s3Client;
    private readonly ILogger<ConfirmConsistencyJob> _logger;

    public ConfirmConsistencyJob(
        IFileRepository fileRepository,
        IAmazonS3 s3Client,
        ILogger<ConfirmConsistencyJob> logger)
    {
        _fileRepository = fileRepository;
        _s3Client = s3Client;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 3, DelaysInSeconds = [5, 10, 15])]
    public async Task Execute(Guid fileId, string key)
    {
        _logger.LogInformation("Start ConfirmConsistencyJob with {fileId} and {key}", fileId, key);

        throw new Exception("buuug!");

        await Task.Delay(3000);
        
        _logger.LogInformation("End ConfirmConsistencyJob");
    }
}