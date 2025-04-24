using FileService.Contracts.SubModels;

// ReSharper disable NotAccessedPositionalProperty.Global
namespace FileService.Contracts.Responses;

public record GetFilesPresignedUrlsResponse(List<ExtendedFileInfo> FilesInfos);