using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Breeds.Queries.GetBreedsBySpeciesId;
using PetFamily.Application.Files.Delete;
using PetFamily.Application.Files.Upload;
using PetFamily.Application.Pets.Queries;
using PetFamily.Application.Pets.Queries.GetFilteredPetsWithPagination;
using PetFamily.Application.Species.Commands.AddBreedToSpecies;
using PetFamily.Application.Species.Commands.Create;
using PetFamily.Application.Species.Commands.DeleteBreedById;
using PetFamily.Application.Species.Commands.DeleteSpeciesById;
using PetFamily.Application.Species.Queries.GetSpeciesWithPagination;
using PetFamily.Application.Volunteers.Commands.AddPet;
using PetFamily.Application.Volunteers.Commands.ChangePetPosition;
using PetFamily.Application.Volunteers.Commands.Create;
using PetFamily.Application.Volunteers.Commands.Delete;
using PetFamily.Application.Volunteers.Commands.DeletePet;
using PetFamily.Application.Volunteers.Commands.DeletePetPhotos;
using PetFamily.Application.Volunteers.Commands.UpdateMainInfo;
using PetFamily.Application.Volunteers.Commands.UpdatePetInfo;
using PetFamily.Application.Volunteers.Commands.UpdatePetMainPhoto;
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
        // from scrutor
        services
            .AddCommands()
            .AddQueries()
            .AddValidatorsFromAssembly(typeof(Inject).Assembly);
        
        return services;
    }

    private static IServiceCollection AddCommands(this IServiceCollection services)
    {
        return services.Scan(scan => scan.FromAssemblies(typeof(Inject).Assembly)
            .AddClasses(classes => classes
                .AssignableToAny(typeof(ICommandHandler<,>), typeof(ICommandHandler<>)))
            .AsSelfWithInterfaces()
            .WithScopedLifetime());
    }
    
    private static IServiceCollection AddQueries(this IServiceCollection services)
    {
        return services.Scan(scan => scan.FromAssemblies(typeof(Inject).Assembly)
            .AddClasses(classes => classes
                .AssignableTo(typeof(IQueryHandler<,>)))
            .AsSelfWithInterfaces()
            .WithScopedLifetime());
    }
}