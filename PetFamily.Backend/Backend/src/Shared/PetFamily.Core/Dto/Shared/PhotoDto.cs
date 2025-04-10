namespace PetFamily.Core.Dto.Shared;

public class PhotoDto
{
    public Guid Id { get; init; }
    public bool Main { get; init; }
    
    public string Url { get; set; } = string.Empty; // получаем в результате запроса к FileService

    public PhotoDto(Guid id, bool main)
    {
        Id = id;
        Main = main;
    }
}