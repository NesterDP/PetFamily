using CSharpFunctionalExtensions;
namespace PetFamily.Domain.Shared.SharedVO;

public record Description
{
    public string Value { get; }

    private Description(string value) => Value = value;

    public static Result<Description> Create(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure<Description>("Description cannot be null or empty.");

        var validDescription = new Description(description);
        
        return Result.Success(validDescription);
    }
}