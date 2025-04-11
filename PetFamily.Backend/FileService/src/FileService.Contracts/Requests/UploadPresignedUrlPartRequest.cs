namespace FileService.Contracts.Requests;

public record UploadPresignedUrlPartRequest(string UploadId, int PartNumber);