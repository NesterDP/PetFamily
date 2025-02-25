namespace PetFamily.API.Controllers.FilesTest.Requests;

public record DeleteFilesRequest(IEnumerable<string> PhotosNames);