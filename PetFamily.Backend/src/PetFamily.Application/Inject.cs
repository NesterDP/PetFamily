using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Files.Delete;
using PetFamily.Application.Files.Upload;
using PetFamily.Application.Species.Commands.AddBreedToSpecies;
using PetFamily.Application.Species.Commands.Create;
using PetFamily.Application.Species.Commands.DeleteBreedById;
using PetFamily.Application.Species.Commands.DeleteSpeciesById;
using PetFamily.Application.Species.Queries.GetBreedsBySpeciesId;
using PetFamily.Application.Species.Queries.GetSpeciesWithPagination;
using PetFamily.Application.Volunteers.Commands.AddPet;
using PetFamily.Application.Volunteers.Commands.ChangePetPosition;
using PetFamily.Application.Volunteers.Commands.Create;
using PetFamily.Application.Volunteers.Commands.Delete;
using PetFamily.Application.Volunteers.Commands.DeletePet;
using PetFamily.Application.Volunteers.Commands.DeletePetPhotos;
using PetFamily.Application.Volunteers.Commands.UpdateMainInfo;
using PetFamily.Application.Volunteers.Commands.UpdatePetInfo;
using PetFamily.Application.Volunteers.Commands.UpdatePetStatus;
using PetFamily.Application.Volunteers.Commands.UpdateSocialNetworks;
using PetFamily.Application.Volunteers.Commands.UpdateTransferDetails;
using PetFamily.Application.Volunteers.Commands.UploadPhotosToPet;
using PetFamily.Application.Volunteers.Queries.GetVolunteerById;
using PetFamily.Application.Volunteers.Queries.GetVolunteersWithPagination;

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
        services.AddScoped<GetVolunteerByIdHandler>();
        services.AddScoped<GetSpeciesWithPaginationHandler>();
        services.AddScoped<GetBreedsBySpeciesIdHandler>();
        
        services.AddScoped<DeleteBreedByIdHandler>();
        services.AddScoped<DeleteSpeciesByIdHandler>();
        
        services.AddScoped<CreateSpeciesHandler>();
        services.AddScoped<AddBreedToSpeciesHandler>();
        
        services.AddScoped<UpdatePetInfoHandler>();
        services.AddScoped<UpdatePetHelpStatusHandler>();
        
        services.AddScoped<SoftDeletePetHandler>();
        services.AddScoped<HardDeletePetHandler>();
        
        services.AddValidatorsFromAssembly(typeof(Inject).Assembly);
        
        return services;
    }
}