namespace PetFamily.Core.Files;

public interface IFilesCleanerService
{
    public Task Process(CancellationToken cancellationToken);
}