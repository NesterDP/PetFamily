using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.PetContext.ValueObjects.PetVO;

public record Color
{
    public string Value { get; }

    private Color(string value) => Value = value;

    public static Result<Color> Create(string color)
    {
        if (string.IsNullOrWhiteSpace(color) || color.Length > Constants.MAX_LOW_TEXT_LENGTH)
            return Result.Failure<Color>("Color is invalid");

        var validColor = new Color(color);
        
        return Result.Success(validColor);
    }
}