using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.SharedKernel.ValueObjects;

public record TransferDetail
{
    public string Name { get; private set; } = null!;

    public string Description { get; private set; } = null!;

    private TransferDetail() { }

    [JsonConstructor]
    private TransferDetail(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public static Result<TransferDetail, Error> Create(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > DomainConstants.MAX_NAME_LENGTH )
            return Errors.General.ValueIsInvalid("name");

        if (string.IsNullOrWhiteSpace(description) || description.Length > DomainConstants.MAX_HIGH_TEXT_LENGTH)
            return Errors.General.ValueIsInvalid("description");

        var validTransferDetail = new TransferDetail(name, description);

        return validTransferDetail;
    }
}