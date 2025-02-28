using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Application.Files.FilesData;

public record FileInfo(FilePath FilePath, string BucketName);