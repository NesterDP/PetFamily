using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared.CustomErrors;

namespace PetFamily.Domain.PetContext.ValueObjects.PetVO;

public record HelpStatus
{
    public PetStatus Value { get; }

    private HelpStatus(PetStatus value) => Value = value;

    public static Result<HelpStatus, Error> Create(PetStatus helpStatus)
    {
        var validHelpStatus = new HelpStatus(helpStatus);
        
        return validHelpStatus;
    }
}

public enum PetStatus
{
    NeedHelp,
    SeekingHome,
    FoundHome
}