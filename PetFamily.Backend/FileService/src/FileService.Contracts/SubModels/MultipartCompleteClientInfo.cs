namespace FileService.Contracts.SubModels;

public record MultipartCompleteClientInfo(string Key, string UploadId, List<PartETagInfo> Parts);