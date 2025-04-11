namespace FileService.Contracts.Requests;

public record GetFilesPresignedUrlsRequest(IEnumerable<Guid> FileIds);