using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.VolunteerRequests.Domain.ValueObjects;

public record VolunteerRequestStatus
{
    public VolunteerRequestStatusEnum Value { get; }

    private VolunteerRequestStatus(VolunteerRequestStatusEnum value) => Value = value;

    public static Result<VolunteerRequestStatus, Error> Create(VolunteerRequestStatusEnum volunteerRequestStatus)
    {
        var validVolunteerRequestStatus = new VolunteerRequestStatus(volunteerRequestStatus);

        return validVolunteerRequestStatus;
    }

    public static Result<VolunteerRequestStatus, Error> Create(string volunteerRequestStatus)
    {
        bool result = Enum.TryParse(volunteerRequestStatus, out VolunteerRequestStatusEnum validStatus);

        if (!result)
            return Errors.General.ValueIsInvalid("volunteerRequestStatus");

        return new VolunteerRequestStatus(validStatus);
    }
}

public enum VolunteerRequestStatusEnum
{
    Submitted,
    OnReview,
    RevisionRequired,
    Rejected,
    Approved
}