using CSharpFunctionalExtensions;
namespace PetFamily.Domain.Shared.SharedVO;

public record Description
{
    public string Value { get; }

    private Description(string value) => Value = value;

    public static Result<Description> Create(string description)
    {
        if (string.IsNullOrWhiteSpace(description) || description.Length > Constants.MAX_HIGH_TEXT_LENGTH)
            return Result.Failure<Description>("Invalid description");

        var validDescription = new Description(description);
        
        return Result.Success(validDescription);
    }
}