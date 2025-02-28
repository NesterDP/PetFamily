using CSharpFunctionalExtensions;
using PetFamily.Application.FilesProvider.FilesData;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.Shared.SharedVO;
using FileInfo = PetFamily.Application.FilesProvider.FilesData.FileInfo;

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
        List<FileInfo> deleteData = new List<FileInfo>();
        foreach (var path in command.PhotosNames)
        {
            deleteData.Add(new FileInfo(FilePath.Create(path).Value, BUCKET_NAME));
        }
        
        var deleteResult = await _filesProvider.DeleteFiles(deleteData, cancellationToken);
            
        if (deleteResult.IsFailure)
            return deleteResult.Error.ToErrorList();
        
        return "done";
    }
}