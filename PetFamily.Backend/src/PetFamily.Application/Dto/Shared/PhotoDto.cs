namespace PetFamily.Application.Dto.Pet;

public class PhotoDto
{
    public string PathToStorage { get; init; } = string.Empty;
    public bool Main { get; init; }
    
    public PhotoDto(string pathToStorage, bool main)
    {
        PathToStorage = pathToStorage;
        Main = main;
    }
}