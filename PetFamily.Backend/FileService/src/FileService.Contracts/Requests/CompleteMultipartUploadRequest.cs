using FileService.Contracts.SubModels;

namespace FileService.Contracts.Requests;

public record CompleteMultipartUploadRequest(List<MultipartCompleteClientInfo> ClientInfos);