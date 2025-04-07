using Microsoft.Extensions.Logging;
using PetFamily.Core;
using PetFamily.Core.Files;
using PetFamily.Core.Messaging;
using PetFamily.Volunteers.Infrastructure.BackgroundServices;
using FileInfo = PetFamily.Core.Files.FilesData.FileInfo;

namespace PetFamily.Volunteers.Infrastructure.Services;

public class FilesCleanerService : IFilesCleanerService
{
    private readonly ILogger<FilesCleanerService> _logger;
    private readonly IMessageQueue<IEnumerable<FileInfo>> _messageQueue;
    private readonly IFilesProvider _filesProvider;

    public FilesCleanerService(
        ILogger<FilesCleanerService> logger,
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