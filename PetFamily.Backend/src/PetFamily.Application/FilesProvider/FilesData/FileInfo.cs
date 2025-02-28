using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Application.FilesProvider.FilesData;

public record FileInfo(FilePath FilePath, string BucketName);