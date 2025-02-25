using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Application.FilesProvider.FilesData;

public record FileData(Stream Stream, FilePath FilePath, string BucketName);