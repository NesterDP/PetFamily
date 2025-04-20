namespace PetFamily.Accounts.Contracts.Dto;

public class AvatarDto
{
    public Guid? Id { get; set; } = null; // получаем из БД сервера (null, если пользователь не загрузил фото)

    public string? Url { get; set; } = null; // получаем в результате запроса к FileService

    public AvatarDto() { }

    public AvatarDto(Guid? guid, string? url)
    {
        Id = guid;
        Url = url;
    }
}