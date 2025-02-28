namespace PetFamily.Application.Files;

public interface IFilesCleanerService
{
    public Task Process(CancellationToken cancellationToken);
}