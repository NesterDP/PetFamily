using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;

public class SocialNetwork
{
    public string Name { get; }
    public string Link { get; }


    private SocialNetwork(string name, string link)
    {
        Name = name;
        Link = link;
    }

    public static Result<SocialNetwork, Error> Create(string link, string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > Constants.MAX_NAME_LENGTH)
            return Errors.General.ValueIsInvalid("name");
        
        if (string.IsNullOrWhiteSpace(link) || link.Length > Constants.MAX_LOW_TEXT_LENGTH)
            return Errors.General.ValueIsInvalid("link");
       
        
        var validSocialNetwork = new SocialNetwork(link, name);
        return validSocialNetwork;
        
    }
}