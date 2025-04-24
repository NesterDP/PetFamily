using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.SharedKernel.ValueObjects;

public record PhotosList
{
    private readonly List<Photo> _photos = null!;
    public IReadOnlyList<Photo> Photos => _photos;

    // ef core
    // ReSharper disable once UnusedMember.Local
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