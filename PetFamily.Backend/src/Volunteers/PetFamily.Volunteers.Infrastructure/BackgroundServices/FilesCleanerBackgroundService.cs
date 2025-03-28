using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Files;

namespace PetFamily.Volunteers.Infrastructure.BackgroundServices;

public class FilesCleanerBackgroundService : BackgroundService
{
    private readonly ILogger<FilesCleanerBackgroundService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public FilesCleanerBackgroundService(
        ILogger<FilesCleanerBackgroundService> logger,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("FilesCleanerBackgroundService is starting to work");
        
        await using var scope = _scopeFactory.CreateAsyncScope();
        var filesCleanerService = scope.ServiceProvider.GetRequiredService<IFilesCleanerService>();

        while (!stoppingToken.IsCancellationRequested)
        {
            await filesCleanerService.Process(stoppingToken);
            _logger.LogInformation("FilesCleanerBackgroundService was used to delete files");
        }
        
        _logger.LogInformation("FilesCleanerBackgroundService is stopping to work");
    }
}