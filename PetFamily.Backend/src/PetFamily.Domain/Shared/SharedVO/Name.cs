using CSharpFunctionalExtensions;
namespace PetFamily.Domain.Shared.SharedVO;

public record Name
{
    public string Value { get; }

    private Name(string value) => Value = value;

    public static Result<Name> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > Constants.MAX_NAME_LENGTH)
            return Result.Failure<Name>("Invalid name");

        var validName = new Name(name);
        
        return Result.Success(validName);
    }
}