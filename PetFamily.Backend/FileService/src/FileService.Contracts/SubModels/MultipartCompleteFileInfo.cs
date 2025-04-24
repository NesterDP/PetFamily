// ReSharper disable NotAccessedPositionalProperty.Global

namespace FileService.Contracts.SubModels;

public record MultipartCompleteFileInfo(Guid FileId, string Key, string ContentType, long ContentLength);