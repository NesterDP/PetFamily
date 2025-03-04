using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared.CustomErrors;

namespace PetFamily.Domain.Shared.SharedVO;

public record Photo
{
    public FilePath PathToStorage { get; }
    
    [JsonConstructor]
    public Photo(FilePath pathToStorage) => PathToStorage = pathToStorage;
    
    public int CompareTo(object? o)
    {
        if(o is Photo photo) return PathToStorage.Path.CompareTo(photo.PathToStorage.Path);
        else throw new ArgumentException("Некорректное значение параметра");
    }
    
}