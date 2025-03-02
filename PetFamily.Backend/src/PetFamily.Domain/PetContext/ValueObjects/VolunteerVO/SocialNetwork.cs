using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.CustomErrors;

namespace PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;

public class SocialNetwork
{
    public string Name { get; }
    public string Link { get; }

    public SocialNetwork() { }
    
    [JsonConstructor]
    private SocialNetwork(string name, string link)
    {
        Name = name;
        Link = link;
    }
    

    public static Result<SocialNetwork, Error> Create(string link, string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > DomainConstants.MAX_NAME_LENGTH)
            return Errors.General.ValueIsInvalid("name");
        
        if (string.IsNullOrWhiteSpace(link) || link.Length > DomainConstants.MAX_LOW_TEXT_LENGTH)
            return Errors.General.ValueIsInvalid("link");
       
        
        var validSocialNetwork = new SocialNetwork(link, name);
        return validSocialNetwork;
        
    }
}