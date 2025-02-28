using PetFamily.Application.Dto.Shared;

namespace PetFamily.Application.Files.Upload;

public record UploadFileCommand(IEnumerable<UploadFileDto> Files);
