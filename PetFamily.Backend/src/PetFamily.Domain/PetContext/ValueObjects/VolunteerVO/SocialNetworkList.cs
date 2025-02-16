using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared.CustomErrors;

namespace PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;

public class SocialNetworkList
{
    private readonly List<SocialNetwork> _socialNetworks;
    public IReadOnlyList<SocialNetwork> SocialNetworks => _socialNetworks;

    // ef core
    private SocialNetworkList() { }

    private SocialNetworkList(IEnumerable<SocialNetwork> socialNetworks)
    {
        _socialNetworks = socialNetworks.ToList();
    }

    public static Result<SocialNetworkList, Error> Create(IEnumerable<SocialNetwork> socialNetworks)
    {
        var validList = new SocialNetworkList(socialNetworks);
        return validList;
    }
}