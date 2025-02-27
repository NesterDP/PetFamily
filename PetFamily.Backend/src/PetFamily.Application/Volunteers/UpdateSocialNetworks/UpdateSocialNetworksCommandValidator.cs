using FluentValidation;
using PetFamily.Application.Extensions;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Application.Volunteers.UpdateSocialNetworks;

public class UpdateSocialNetworksCommandValidator : AbstractValidator<UpdateSocialNetworksCommand>
{
    public UpdateSocialNetworksCommandValidator()
    {
        RuleFor(r => r.Id).NotEmpty().WithError(Errors.General.ValueIsRequired());
        RuleForEach(r => r.Dto.SocialNetworks)
            .MustBeValueObject(s => SocialNetwork.Create(s.Name, s.Link));
    }
} 