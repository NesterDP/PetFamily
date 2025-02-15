using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.PetContext.ValueObjects.PetVO;

public record IsCastrated
{
    public bool Value { get; }

    private IsCastrated(bool value) => Value = value;

    public static Result<IsCastrated, Error> Create(bool isCastrated)
    {
        var validIsCastrated = new IsCastrated(isCastrated);
        
        return validIsCastrated;
    }
}