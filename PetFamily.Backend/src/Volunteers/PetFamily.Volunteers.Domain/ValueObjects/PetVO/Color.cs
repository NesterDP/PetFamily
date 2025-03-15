using CSharpFunctionalExtensions;
using PetFamily.SharedKernel;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Volunteers.Domain.ValueObjects.PetVO;

public record Color
{
    public string Value { get; }

    private Color(string value) => Value = value;

    public static Result<Color, Error> Create(string color)
    {
        if (string.IsNullOrWhiteSpace(color) || color.Length > DomainConstants.MAX_LOW_TEXT_LENGTH)
            return Errors.General.ValueIsInvalid("color");

        var validColor = new Color(color);
        
        return validColor;
    }
}