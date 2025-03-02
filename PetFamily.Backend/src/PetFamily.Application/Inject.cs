using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Files.Delete;
using PetFamily.Application.Files.Upload;
using PetFamily.Application.Volunteers.Queries.GetVolunteersWithPagination;
using PetFamily.Application.Volunteers.UseCases.AddPet;
using PetFamily.Application.Volunteers.UseCases.ChangePetPosition;
using PetFamily.Application.Volunteers.UseCases.Create;
using PetFamily.Application.Volunteers.UseCases.Delete;
using PetFamily.Application.Volunteers.UseCases.DeletePetPhotos;
using PetFamily.Application.Volunteers.UseCases.UpdateMainInfo;
using PetFamily.Application.Volunteers.UseCases.UpdateSocialNetworks;
using PetFamily.Application.Volunteers.UseCases.UpdateTransferDetails;
using PetFamily.Application.Volunteers.UseCases.UploadPhotosToPet;

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

        services.AddScoped<GetVolunteersWithPaginationHandler>();
        services.AddValidatorsFromAssembly(typeof(Inject).Assembly);
        
        return services;
    }
}