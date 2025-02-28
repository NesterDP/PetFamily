using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.FilesProvider.Delete;
using PetFamily.Application.FilesProvider.Upload;
using PetFamily.Application.Volunteers.AddPet;
using PetFamily.Application.Volunteers.ChangePetPosition;
using PetFamily.Application.Volunteers.Create;
using PetFamily.Application.Volunteers.Delete;
using PetFamily.Application.Volunteers.DeletePetPhotos;
using PetFamily.Application.Volunteers.UpdateMainInfo;
using PetFamily.Application.Volunteers.UpdateSocialNetworks;
using PetFamily.Application.Volunteers.UpdateTransferDetails;
using PetFamily.Application.Volunteers.UploadPhotosToPet;

namespace PetFamily.Application;

public static class Inject
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateVolunteerHandler>();
        services.AddScoped<UpdateMainInfoHandler>();
        services.AddScoped<UpdateSocialNetworksHandler>();
        services.AddScoped<UpdateTransferDetailsHandler>();
        services.AddScoped<HardDeleteVolunteerHandler>();
        services.AddScoped<SoftDeleteVolunteerHandler>();

        services.AddScoped<UploadFilesHandler>();
        services.AddScoped<DeleteFilesHandler>();
        services.AddScoped<UploadPhotosToPetHandler>();
        services.AddScoped<ChangePetPositionHandler>();
        
        services.AddScoped<AddPetHandler>();
        services.AddScoped<DeletePetPhotosHandler>();
        services.AddValidatorsFromAssembly(typeof(Inject).Assembly);
        
        return services;
    }
}