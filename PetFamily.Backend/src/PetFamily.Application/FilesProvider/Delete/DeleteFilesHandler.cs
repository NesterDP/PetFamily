using CSharpFunctionalExtensions;
using PetFamily.Application.FilesProvider.FilesData;
using PetFamily.Application.Providers;
using PetFamily.Domain.Shared.CustomErrors;

namespace PetFamily.Application.FilesProvider.Delete;

public class DeleteFilesHandler
{
    private readonly IFilesProvider _filesProvider;
    private const string BUCKET_NAME = "TEST";

    public DeleteFilesHandler(IFilesProvider filesProvider)
    {
        _filesProvider = filesProvider;
    }

    public async Task<Result<string, ErrorList>> HandleAsync(
        DeleteFilesCommand command,
        CancellationToken cancellationToken)
    {
        List<DeleteData> deleteData = new List<DeleteData>();
        foreach (var path in command.PhotosNames)
        {
            deleteData.Add(new DeleteData(path, BUCKET_NAME));
        }
        
        var deleteResult = await _filesProvider.DeleteFiles(deleteData, cancellationToken);
            
        if (deleteResult.IsFailure)
            return deleteResult.Error.ToErrorList();
        
        return "done";
    }
}