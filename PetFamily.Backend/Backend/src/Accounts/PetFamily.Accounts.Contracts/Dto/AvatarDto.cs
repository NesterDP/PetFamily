namespace PetFamily.Accounts.Contracts.Dto;

public class AvatarDto
{
    public Guid? Id { get; set; } // получаем из БД сервера (null, если пользователь не загрузил фото)

    public string? Url { get; set; } // получаем в результате запроса к FileService

    public AvatarDto() { }

    public AvatarDto(Guid? guid, string? url)
    {
        Id = guid;
        Url = url;
    }
}