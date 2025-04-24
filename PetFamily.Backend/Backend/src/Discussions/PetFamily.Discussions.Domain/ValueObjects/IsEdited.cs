using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Discussions.Domain.ValueObjects;

public record IsEdited
{
    public bool Value { get; }

    private IsEdited(bool value) => Value = value;

    public static Result<IsEdited, Error> Create(bool isEdited)
    {
        var validIsEdited = new IsEdited(isEdited);

        return validIsEdited;
    }
}