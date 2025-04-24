using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Discussions.Domain.ValueObjects;

public record DiscussionStatus
{
    public DiscussionStatusEnum Value { get; }

    private DiscussionStatus(DiscussionStatusEnum value) => Value = value;

    public static Result<DiscussionStatus, Error> Create(DiscussionStatusEnum discussionStatus)
    {
        var validDiscussionStatus = new DiscussionStatus(discussionStatus);
        
        return validDiscussionStatus;
    }
    
    public static Result<DiscussionStatus, Error> Create(string discussionStatus)
    {
        bool result = Enum.TryParse(discussionStatus, out DiscussionStatusEnum validStatus);

        if (!result)
            return  Errors.General.ValueIsInvalid("DiscussionStatus");
        
        return new DiscussionStatus(validStatus);
    }
}

public enum DiscussionStatusEnum
{
    Opened,
    Closed
}