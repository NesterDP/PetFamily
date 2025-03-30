using FluentValidation;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.VolunteerRequests.Domain.ValueObjects;

namespace PetFamily.VolunteerRequests.Application.Commands.CreateVolunteerRequest;


public class CreateVolunteerRequestCommandValidator : AbstractValidator<CreateVolunteerRequestCommand>
{
    public CreateVolunteerRequestCommandValidator()
    {
        RuleFor(c => c.VolunteerInfo).MustBeValueObject(VolunteerInfo.Create);
    }
}