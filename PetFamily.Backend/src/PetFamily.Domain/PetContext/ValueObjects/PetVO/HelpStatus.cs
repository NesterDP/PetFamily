using CSharpFunctionalExtensions;
namespace PetFamily.Domain.PetContext.ValueObjects.PetVO;

public record HelpStatus
{
    public PetStatus Value { get; }

    private HelpStatus(PetStatus value) => Value = value;

    public static Result<HelpStatus> Create(PetStatus helpStatus)
    {
        var validHelpStatus = new HelpStatus(helpStatus);
        
        return Result.Success(validHelpStatus);
    }
}

public enum PetStatus
{
    NeedHelp,
    SeekingHome,
    FoundHome
}