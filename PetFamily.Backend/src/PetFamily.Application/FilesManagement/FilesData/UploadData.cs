namespace PetFamily.Application.FilesManagement.FilesData;

public record UploadData(Stream Stream, string BucketName, string ObjectName);