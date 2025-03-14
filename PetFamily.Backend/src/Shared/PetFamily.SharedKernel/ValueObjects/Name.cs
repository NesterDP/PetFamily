using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.SharedKernel.ValueObjects;


public record Name
{
    public string Value { get; }

    private Name(string value) => Value = value;

    public static Result<Name, Error> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > SharedConstants.MAX_NAME_LENGTH)
            return Errors.General.ValueIsInvalid("name");

        var validName = new Name(name);
        
        return validName;
    }
}