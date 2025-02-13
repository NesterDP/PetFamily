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
        if (string.IsNullOrWhiteSpace(name) ||  name.Length > Constants.MAX_NAME_LENGTH )
            return Result.Failure<TransferDetails>("Transfer details name is invalid");
        
        if (string.IsNullOrWhiteSpace(description) || description.Length > Constants.MAX_HIGH_TEXT_LENGTH)
            return Result.Failure<TransferDetails>("Description of TransferDetails is invalid");

        var validTransferDetails = new TransferDetails(name, description);
        
        return Result.Success(validTransferDetails);
    }
}