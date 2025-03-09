using FluentValidation;
using PetFamily.Application.Extensions;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared.CustomErrors;

namespace PetFamily.Application.Volunteers.Commands.UpdateSocialNetworks;

public class UpdateSocialNetworksCommandValidator : AbstractValidator<UpdateSocialNetworksCommand>
{
    public UpdateSocialNetworksCommandValidator()
    {
        RuleFor(r => r.Id).NotEmpty().WithError(Errors.General.ValueIsRequired());
        RuleForEach(r => r.SocialNetworks)
            .MustBeValueObject(s => SocialNetwork.Create(s.Name, s.Link));
    }
} 