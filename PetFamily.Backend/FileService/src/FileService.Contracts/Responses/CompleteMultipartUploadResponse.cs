using FileService.Contracts.SubModels;

namespace FileService.Contracts.Responses;

public record CompleteMultipartUploadResponse(List<MultipartCompleteFileInfo> MultipartCompleteInfos);