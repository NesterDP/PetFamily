namespace PetFamily.Accounts.Contracts.Dto;

public record SocialNetworkDto
{
    public SocialNetworkDto(string Name, string Link)
    {
        this.Name = Name;
        this.Link = Link;
    }

    public string Name { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;

    public void Deconstruct(out string name, out string link)
    {
        name = this.Name;
        link = this.Link;
    }
}