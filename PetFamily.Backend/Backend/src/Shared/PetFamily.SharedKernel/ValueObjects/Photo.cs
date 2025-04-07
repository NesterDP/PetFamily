using System.Text.Json.Serialization;

namespace PetFamily.SharedKernel.ValueObjects;

public record Photo
{
    public FilePath PathToStorage { get; }
    
    public bool Main { get; } = false;
    
    public Photo(FilePath pathToStorage) => PathToStorage = pathToStorage;

    [JsonConstructor]
    public Photo(FilePath pathToStorage, bool main)
    {
        PathToStorage = pathToStorage;
        Main = main;
    }

    public Photo CreateCopy(bool main)
    {
        return new Photo(FilePath.Create(PathToStorage.Path).Value, main);
    }
    
    public Photo CreateCopy()
    {
        return new Photo(FilePath.Create(PathToStorage.Path).Value, Main);
    }
    
    
    
    public int CompareTo(object? o)
    {
        if(o is Photo photo) return PathToStorage.Path.CompareTo(photo.PathToStorage.Path);
        throw new ArgumentException("Incorrect value of compared item");
    }



}