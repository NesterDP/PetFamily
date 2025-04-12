using FileService.Contracts.SubModels;

namespace FileService.Contracts.Responses;

public record StartMultipartUploadResponse(List<MultipartStartProviderInfo> MultipartStartInfos);