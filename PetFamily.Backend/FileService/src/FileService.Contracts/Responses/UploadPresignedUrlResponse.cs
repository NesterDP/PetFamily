namespace FileService.Contracts.Responses;

public record UploadPresignedUrlResponse(string Key, string Url);