using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.Structs;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace PetFamily.Volunteers.Application.Commands.UpdateSocialNetworks;

public class UpdateSocialNetworksHandler : ICommandHandler<Guid, UpdateSocialNetworksCommand>
{
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly ILogger<UpdateSocialNetworksHandler> _logger;
    private readonly IValidator<UpdateSocialNetworksCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSocialNetworksHandler(
        IVolunteersRepository volunteersRepository,
        ILogger<UpdateSocialNetworksHandler> logger,
        IValidator<UpdateSocialNetworksCommand> validator,
        [FromKeyedServices(UnitOfWorkSelector.Volunteers)]
        IUnitOfWork unitOfWork)
    {
        _volunteersRepository = volunteersRepository;
        _logger = logger;
        _validator = validator;
        _unitOfWork = unitOfWork;
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
        foreach (var socialNetwork in command.SocialNetworks)
        {
            var tempResult = SocialNetwork.Create(socialNetwork.Name, socialNetwork.Link);
            socialNetworksList.Add(tempResult.Value);
        }

        // volunteerResult.Value.UpdateSocialNetworks(socialNetworksList);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Volunteer was updated (social networks), his ID = {ID}", volunteerId.Value);

        return volunteerId.Value;
    }
}