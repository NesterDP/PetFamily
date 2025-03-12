using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;
using PetFamily.Core.CustomErrors;

namespace PetFamily.Core.SharedVO;

public class FilePath
{
    public string Path { get; }
    

    [JsonConstructor]
    private FilePath(string path)
    {
        Path = path;
    }

    public static Result<FilePath, Error> Create(Guid path, string extension)
    {
        //if (string.IsNullOrWhiteSpace(extension) || extension.Length > Constants.MAX_EXTENSION_LENGTH)
            //return Result.Failure<FilePath, Error>(Errors.General.ValueIsInvalid("file extension"));
        
        var fullPath = path + extension;
        
        return new FilePath(fullPath);
    }
    
    public static Result<FilePath, Error> Create(string fullPath)
    {
        
        return new FilePath(fullPath);
    }
}
