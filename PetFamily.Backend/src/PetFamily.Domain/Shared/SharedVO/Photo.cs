using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared.CustomErrors;

namespace PetFamily.Domain.Shared.SharedVO;

public record Photo
{
    public FilePath PathToStorage { get; }
    
    public int Priority { get; } = 0;
    
    public Photo(FilePath pathToStorage) => PathToStorage = pathToStorage;

    [JsonConstructor]
    public Photo(FilePath pathToStorage, int priority)
    {
        PathToStorage = pathToStorage;
        Priority = priority;
    }

    public Photo CreateCopy(int priority)
    {
        return new Photo(FilePath.Create(PathToStorage.Path).Value, priority);
    }
    
    public Photo CreateCopy()
    {
        return new Photo(FilePath.Create(PathToStorage.Path).Value, Priority);
    }
    
    
    
    public int CompareTo(object? o)
    {
        if(o is Photo photo) return PathToStorage.Path.CompareTo(photo.PathToStorage.Path);
        else throw new ArgumentException("Некорректное значение параметра");
    }



}