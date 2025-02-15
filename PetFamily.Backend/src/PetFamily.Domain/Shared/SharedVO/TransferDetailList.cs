using CSharpFunctionalExtensions;
namespace PetFamily.Domain.Shared.SharedVO;

public record TransferDetailList
{
    private readonly List<TransferDetail> _transferDetails;
    public IReadOnlyList<TransferDetail> TransferDetails => _transferDetails;
    
    // ef core
    private TransferDetailList() { }
    
    private TransferDetailList(IEnumerable<TransferDetail> transferDetails)
    {
        _transferDetails = transferDetails.ToList();
    }

    public static Result<TransferDetailList, Error> Create(IEnumerable<TransferDetail> transferDetails)
    {
        return new TransferDetailList(transferDetails);
    }
}