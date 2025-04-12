namespace PetFamily.Core.Dto.Shared;

public record CompleteUploadFileDto(string Key, string UploadId, List<PartETagInfoDto> Parts);