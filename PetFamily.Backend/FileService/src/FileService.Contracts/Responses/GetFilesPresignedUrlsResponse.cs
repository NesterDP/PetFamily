using FileService.Contracts.SubModels;

namespace FileService.Contracts.Responses;

public record GetFilesPresignedUrlsResponse(List<ExtendedFileInfo> FilesInfos);