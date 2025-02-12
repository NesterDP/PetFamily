using CSharpFunctionalExtensions;
namespace PetFamily.Domain.PetContext.ValueObjects.PetVO;

public record IsCastrated
{
    public bool Value { get; }

    private IsCastrated(bool value) => Value = value;

    public static Result<IsCastrated> Create(bool isCastrated)
    {
        var validIsCastrated = new IsCastrated(isCastrated);
        
        return Result.Success(validIsCastrated);
    }
}