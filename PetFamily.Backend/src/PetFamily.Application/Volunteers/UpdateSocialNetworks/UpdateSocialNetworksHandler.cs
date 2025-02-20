using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared.CustomErrors;

namespace PetFamily.Application.Volunteers.UpdateSocialNetworks;

public class UpdateSocialNetworksHandler
{
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly ILogger<UpdateSocialNetworksHandler> _logger;

    public UpdateSocialNetworksHandler(
        IVolunteersRepository volunteersRepository,
        ILogger<UpdateSocialNetworksHandler> logger)
    {
        _volunteersRepository = volunteersRepository;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> HandleAsync(
        UpdateSocialNetworksRequest request,
        CancellationToken cancellationToken)
    {
        var volunteerId = VolunteerId.Create(request.Id);

        var volunteerResult = await _volunteersRepository.GetByIdAsync(volunteerId, cancellationToken);
        if (volunteerResult.IsFailure)
            return volunteerResult.Error;

        List<SocialNetwork> socialNetworkList = [];
        foreach (var socialNetwork in request.Dto.SocialNetworks)
        {
            var tempResult = SocialNetwork.Create(socialNetwork.Name, socialNetwork.Link);
            socialNetworkList.Add(tempResult.Value);
        }

        volunteerResult.Value.UpdateSocialNetworks(SocialNetworkList.Create(socialNetworkList).Value);

        var result = await _volunteersRepository.SaveAsync(volunteerResult.Value, cancellationToken);

        _logger.LogInformation(
            "Volunteer was updated (social networks), his ID = {ID}", volunteerId.Value);

        return volunteerId.Value;
    }
}