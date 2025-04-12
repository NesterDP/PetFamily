using FileService.Contracts.SubModels;

namespace FileService.Contracts.Requests;

public record StartMultipartUploadRequest(List<MultipartStartClientInfo> StartMultipartClientInfos);