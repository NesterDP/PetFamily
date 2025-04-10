using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace PetFamily.SharedKernel.ValueObjects;

public record Avatar
{
    public static readonly string[] AllowedTypes =  [DomainConstants.PNG, DomainConstants.JPG, DomainConstants.WEBP];
    public FileId? Id { get; } = null; // null, если пользователь не загрузил фото
    
    private Avatar(FileId id) => Id = id;

    public static Result<Avatar, Error> Create(Guid fileId, string fileType)
    {
        if (!AllowedTypes.Contains(fileType))
            return Errors.General.ValueIsInvalid("fileType");

        return new Avatar(fileId);
    }
}