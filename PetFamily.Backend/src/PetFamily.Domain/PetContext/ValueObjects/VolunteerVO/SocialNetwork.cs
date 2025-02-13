using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;

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
        if (string.IsNullOrWhiteSpace(name) || name.Length > Constants.MAX_NAME_LENGTH)
            return Result.Failure<SocialNetwork>("Invalid link's name");
        
        if (string.IsNullOrWhiteSpace(link) || link.Length > Constants.MAX_LOW_TEXT_LENGTH)
            return Result.Failure<SocialNetwork>("link is either empty or too long");
       
        
        var validSocialNetwork = new SocialNetwork(link, name);
        return Result.Success(validSocialNetwork);
        
    }
}