using CSharpFunctionalExtensions;
namespace PetFamily.Domain.Shared.SharedVO;

public record Name
{
    public string Value { get; }

    private Name(string value) => Value = value;

    public static Result<Name> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Name>("Name cannot be null or empty.");

        var validName = new Name(name);
        
        return Result.Success(validName);
    }
}