using CSharpFunctionalExtensions;
namespace PetFamily.Domain.Shared.SharedVO;

public record TransferDetails
{
    public string Name { get; private set; }
    public string Description { get; private set;}

    private TransferDetails(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public static Result<TransferDetails> Create(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<TransferDetails>("Name of TransferDetails cannot be null or empty.");
        
        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure<TransferDetails>("Description of TransferDetails cannot be null or empty.");

        var validTransferDetails = new TransferDetails(name, description);
        
        return Result.Success(validTransferDetails);
    }
}