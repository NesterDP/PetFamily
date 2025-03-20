using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.SharedKernel.ValueObjects;

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