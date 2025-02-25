using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared.CustomErrors;

namespace PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;

public class SocialNetworksList
{
    private readonly List<SocialNetwork> _socialNetworks;
    public IReadOnlyList<SocialNetwork> SocialNetworks => _socialNetworks;

    // ef core
    private SocialNetworksList() { }

    private SocialNetworksList(IEnumerable<SocialNetwork> socialNetworks)
    {
        _socialNetworks = socialNetworks.ToList();
    }

    public static Result<SocialNetworksList, Error> Create(IEnumerable<SocialNetwork> socialNetworks)
    {
        var validList = new SocialNetworksList(socialNetworks);
        return validList;
    }
}