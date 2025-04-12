namespace FileService.Contracts.Requests;

public record DeleteFilesByIdsRequest(List<Guid> Ids);