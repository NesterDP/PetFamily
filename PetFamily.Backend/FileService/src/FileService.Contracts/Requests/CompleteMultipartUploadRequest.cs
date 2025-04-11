using FileService.Contracts.SubModels;

namespace FileService.Contracts.Requests;

public record CompleteMultipartUploadRequest(string UploadId, List<PartETagInfo> Parts);