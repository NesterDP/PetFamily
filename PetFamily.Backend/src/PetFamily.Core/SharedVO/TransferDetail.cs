using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;
using PetFamily.Core.CustomErrors;
using PetFamily.Core.GeneralClasses;

namespace PetFamily.Core.SharedVO;

public record TransferDetail
{
    public string Name { get; private set; }
    public string Description { get; private set;}
    
    TransferDetail() {}
    
    [JsonConstructor]
    private TransferDetail(string name, string description)
    {
        Name = name;
        Description = description;
    }
    

    public static Result<TransferDetail, Error> Create(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name) ||  name.Length > DomainConstants.MAX_NAME_LENGTH )
            return Errors.General.ValueIsInvalid("name");
        
        if (string.IsNullOrWhiteSpace(description) || description.Length > DomainConstants.MAX_HIGH_TEXT_LENGTH)
            return Errors.General.ValueIsInvalid("description");

        var validTransferDetail = new TransferDetail(name, description);
        
        return validTransferDetail;
    }
}