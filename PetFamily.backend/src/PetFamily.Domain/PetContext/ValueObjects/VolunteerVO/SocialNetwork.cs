using CSharpFunctionalExtensions;

namespace PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;

public class SocialNetwork
{
    public string Link { get; }
    public string Name { get; }

    private SocialNetwork(string link, string name)
    {
        Link = link;
        Name = name;
    }

    public static Result<SocialNetwork> Create(string link, string name)
    {
        if (string.IsNullOrWhiteSpace(link))
            return Result.Failure<SocialNetwork>("Link cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<SocialNetwork>("Link's name cannot be null or empty.");
        
        var validSocialNetwork = new SocialNetwork(link, name);
        return Result.Success(validSocialNetwork);
        
    }
}