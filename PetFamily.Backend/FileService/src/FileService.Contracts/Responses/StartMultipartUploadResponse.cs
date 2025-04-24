using FileService.Contracts.SubModels;

// ReSharper disable NotAccessedPositionalProperty.Global
namespace FileService.Contracts.Responses;

public record StartMultipartUploadResponse(List<MultipartStartProviderInfo> MultipartStartInfos);