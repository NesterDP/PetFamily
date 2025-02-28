using Microsoft.Extensions.Logging;
using PetFamily.Application.Files;
using PetFamily.Application.Messaging;
using PetFamily.Infrastructure.BackgroundServices;
using FileInfo = PetFamily.Application.Files.FilesData.FileInfo;

namespace PetFamily.Infrastructure.Files;

public class FilesCleanerService : IFilesCleanerService
{
    private readonly ILogger<FilesCleanerBackgroundService> _logger;
    private readonly IMessageQueue<IEnumerable<FileInfo>> _messageQueue;
    private readonly IFilesProvider _filesProvider;

    public FilesCleanerService(
        ILogger<FilesCleanerBackgroundService> logger,
        IMessageQueue<IEnumerable<FileInfo>> messageQueue,
        IFilesProvider filesProvider)
    {
        _logger = logger;
        _messageQueue = messageQueue;
        _filesProvider = filesProvider;
    }

    public async Task Process(CancellationToken cancellationToken)
    {
        var fileInfos = await _messageQueue.ReadAsync(cancellationToken);
        await _filesProvider.DeleteFiles(fileInfos, cancellationToken);
    }
}