// ReSharper disable NotAccessedPositionalProperty.Global

namespace FileService.Contracts.Responses;

public record DeleteFilesByIdsResponse(List<string> Keys);