using PetFamily.Application.Dto.Shared;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Application.FilesManagement.Upload;

public record UploadFileCommand(IEnumerable<CreateFileDto> Files);
