using CSharpFunctionalExtensions;
namespace PetFamily.Domain.PetContext.ValueObjects.PetVO;

public record Color
{
    public string Value { get; }

    private Color(string value) => Value = value;

    public static Result<Color> Create(string color)
    {
        if (string.IsNullOrWhiteSpace(color))
            return Result.Failure<Color>("Color cannot be null or empty.");

        var validColor = new Color(color);
        
        return Result.Success(validColor);
    }
}