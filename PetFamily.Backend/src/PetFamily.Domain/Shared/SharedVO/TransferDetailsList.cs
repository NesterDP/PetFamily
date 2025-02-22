using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared.CustomErrors;
namespace PetFamily.Domain.Shared.SharedVO;

public record TransferDetailsList
{
    private readonly List<TransferDetail> _transferDetails;
    public IReadOnlyList<TransferDetail> TransferDetails => _transferDetails;
    
    // ef core
    private TransferDetailsList() { }
    
    private TransferDetailsList(IEnumerable<TransferDetail> transferDetails)
    {
        _transferDetails = transferDetails.ToList();
    }

    public static Result<TransferDetailsList, Error> Create(IEnumerable<TransferDetail> transferDetails)
    {
        return new TransferDetailsList(transferDetails);
    }
}