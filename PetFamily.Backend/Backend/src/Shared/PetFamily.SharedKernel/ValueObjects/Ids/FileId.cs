using CSharpFunctionalExtensions;

namespace PetFamily.SharedKernel.ValueObjects.Ids;

public class FileId : ValueObject, IComparable<FileId>
{
    public Guid Value { get; }

    private FileId(Guid value) => Value = value;

    public static FileId NewFileId() => new(Guid.NewGuid());

    public static FileId EmptyFileId => new(Guid.Empty);

    public static FileId Create(Guid id) => new(id);

    public static implicit operator FileId(Guid fileId) => new(fileId);

    public static implicit operator Guid(FileId fileId) => fileId.Value;

    public int CompareTo(FileId? other)
    {
        if (other == null)
            throw new Exception("FileId cannot be null");
        return Value.CompareTo(other.Value);
    }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Value;
    }
}