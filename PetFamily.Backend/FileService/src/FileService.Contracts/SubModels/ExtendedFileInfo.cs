namespace FileService.Contracts.SubModels;

public record ExtendedFileInfo(
    Guid Id,
    string Key,
    string Url,
    DateTime UploadDate,
    long Size,
    string ContentType);