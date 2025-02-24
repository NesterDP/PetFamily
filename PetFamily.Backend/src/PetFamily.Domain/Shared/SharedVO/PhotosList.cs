using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared.CustomErrors;

namespace PetFamily.Domain.Shared.SharedVO;

public record PhotosList
{
    private readonly List<Photo> _photos;
    public IReadOnlyList<Photo> Photos => _photos;
    
    // ef core
    private PhotosList() { }
    
    private PhotosList(IEnumerable<Photo> photos)
    {
        _photos = photos.ToList();
    }

    public static Result<PhotosList, Error> Create(IEnumerable<Photo> photos)
    {
        return new PhotosList(photos);
    }
}