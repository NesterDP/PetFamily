using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.ValueObjects.Ids;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.CustomErrors;


namespace PetFamily.SharedKernel.ValueObjects;

public record Photo
{
    public static readonly string[] AllowedTypes = [DomainConstants.PNG, DomainConstants.JPG, DomainConstants.WEBP];

    public FileId Id { get; }
    public bool Main { get; } = false;
    
    private Photo(FileId id) => Id = id;
    
    [JsonConstructor]
    private Photo(FileId id, bool main)
    {
        Id = id;
        Main = main;
    }
    
    public static Result<Photo, Error> Create(Guid fileId, string fileType)
    {
        if (!AllowedTypes.Contains(fileType))
            return Errors.General.ValueIsInvalid("fileType");

        return new Photo(fileId);
    }
    
    public static Result<Photo, Error> Create(Guid fileId, bool main, string fileType)
    {
        if (!AllowedTypes.Contains(fileType))
            return Errors.General.ValueIsInvalid("fileType");

        return new Photo(fileId, main);
    }
    
    public Photo CreateCopy(bool main)
    {
        return new Photo(Id, main);
    }
    
    public Photo CreateCopy()
    {
        return new Photo(Id, Main);
    }
    
    public int CompareTo(object? o)
    {
        if(o is Photo photo) return Id.CompareTo(photo.Id);
        throw new ArgumentException("Incorrect value of compared item");
    }
}