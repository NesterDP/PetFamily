using PetFamily.SharedKernel.ValueObjects;

namespace PetFamily.Core.Files.FilesData;

public record FileInfo(FilePath FilePath, string BucketName);