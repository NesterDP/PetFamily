using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Extensions;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared.CustomErrors;

namespace PetFamily.Application.Volunteers.UpdateSocialNetworks;

public class UpdateSocialNetworksHandler
{
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly ILogger<UpdateSocialNetworksHandler> _logger;
    private readonly IValidator<UpdateSocialNetworksCommand> _validator;

    public UpdateSocialNetworksHandler(
        IVolunteersRepository volunteersRepository,
        ILogger<UpdateSocialNetworksHandler> logger,
        IValidator<UpdateSocialNetworksCommand> validator)
    {
        _volunteersRepository = volunteersRepository;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        UpdateSocialNetworksCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToErrorList();
        
        var volunteerId = VolunteerId.Create(command.Id);

        var volunteerResult = await _volunteersRepository.GetByIdAsync(volunteerId, cancellationToken);
        if (volunteerResult.IsFailure)
            return volunteerResult.Error.ToErrorList();

        List<SocialNetwork> socialNetworksList = [];
        foreach (var socialNetwork in command.Dto.SocialNetworks)
        {
            var tempResult = SocialNetwork.Create(socialNetwork.Name, socialNetwork.Link);
            socialNetworksList.Add(tempResult.Value);
        }

        volunteerResult.Value.UpdateSocialNetworks(SocialNetworksList.Create(socialNetworksList).Value);

        var result = await _volunteersRepository.SaveAsync(volunteerResult.Value, cancellationToken);

        _logger.LogInformation(
            "Volunteer was updated (social networks), his ID = {ID}", volunteerId.Value);

        return volunteerId.Value;
    }
}