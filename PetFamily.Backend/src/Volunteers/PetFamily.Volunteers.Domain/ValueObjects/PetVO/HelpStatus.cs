using CSharpFunctionalExtensions;
using PetFamily.Core.CustomErrors;

namespace PetFamily.Volunteers.Domain.ValueObjects.PetVO;

public record HelpStatus
{
    public PetStatus Value { get; }

    private HelpStatus(PetStatus value) => Value = value;

    public static Result<HelpStatus, Error> Create(PetStatus helpStatus)
    {
        var validHelpStatus = new HelpStatus(helpStatus);
        
        return validHelpStatus;
    }
    
    public static Result<HelpStatus, Error> Create(string helpStatus)
    {
        var result = Enum.TryParse(helpStatus, out PetStatus myStatus);

        if (!result)
           return  Errors.General.ValueIsInvalid("helpStatus");
        
        return new HelpStatus(myStatus);
    }
}

public enum PetStatus
{
    InSearchOfHome,
    UnderMedicalTreatment,
    FoundHome
}