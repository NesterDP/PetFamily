using Microsoft.AspNetCore.Http;

namespace PetFamily.Volunteers.Presentation.Volunteers.Requests;

public record UploadPhotosToPetRequest(IFormFileCollection Files);