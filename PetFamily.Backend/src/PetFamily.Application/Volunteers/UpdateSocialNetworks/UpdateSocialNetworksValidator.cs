using FluentValidation;
using PetFamily.API.Extensions;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Application.Volunteers.UpdateSocialNetworks;

public class UpdateSocialNetworksValidator : AbstractValidator<UpdateSocialNetworksRequest>
{
    public UpdateSocialNetworksValidator()
    {
        RuleFor(r => r.Id).NotEmpty().WithError(Errors.General.ValueIsRequired());
        RuleForEach(r => r.Dto.SocialNetworks)
            .MustBeValueObject(s => SocialNetwork.Create(s.Name, s.Link));
    }
} 