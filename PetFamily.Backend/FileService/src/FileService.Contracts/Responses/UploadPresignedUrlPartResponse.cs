// ReSharper disable NotAccessedPositionalProperty.Global

namespace FileService.Contracts.Responses;

public record UploadPresignedUrlPartResponse(string Key, string Url);