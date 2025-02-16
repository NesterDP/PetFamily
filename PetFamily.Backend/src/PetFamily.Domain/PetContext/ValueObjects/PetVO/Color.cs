using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.CustomErrors;

namespace PetFamily.Domain.PetContext.ValueObjects.PetVO;

public record Color
{
    public string Value { get; }

    private Color(string value) => Value = value;

    public static Result<Color, Error> Create(string color)
    {
        if (string.IsNullOrWhiteSpace(color) || color.Length > Constants.MAX_LOW_TEXT_LENGTH)
            return Errors.General.ValueIsInvalid("color");

        var validColor = new Color(color);
        
        return validColor;
    }
}