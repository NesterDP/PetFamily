using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.Volunteers.Application;
using PetFamily.Volunteers.Infrastructure.DbContexts;
using PetFamily.Volunteers.Infrastructure.Repositories;
using Minio;
using PetFamily.Core.Files;
using PetFamily.Core.Messaging;
using PetFamily.Core;
using PetFamily.Volunteers.Infrastructure.BackgroundServices;
using PetFamily.Volunteers.Infrastructure.Files;
using PetFamily.Volunteers.Infrastructure.MessageQueues;
using PetFamily.Volunteers.Infrastructure.Providers;
using FileInfo = PetFamily.Core.Files.FilesData.FileInfo;
using MinioOptions = PetFamily.Volunteers.Infrastructure.Options.MinioOptions;

namespace PetFamily.Volunteers.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddVolunteersInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDbContexts(configuration)
            .AddMinio(configuration)
            .AddRepositories()
            .AddTransactionManagement()
            .AddHostedServices()
            .AddMessageQueues()
            .AddServices();
        return services;
    }
    
    private static IServiceCollection AddMinio(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MinioOptions>(
            configuration.GetSection(MinioOptions.MINIO));

        services.AddMinio(options =>
        {
            var minioOptions = configuration.GetSection(MinioOptions.MINIO).Get<MinioOptions>()
                               ?? throw new ApplicationException("Missing minio configuration");

            options.WithEndpoint(minioOptions.Endpoint);
            options.WithCredentials(minioOptions.AccessKey, minioOptions.SecretKey);
            options.WithSSL(minioOptions.WithSsL);
        });
        
        
        services.AddScoped<IFilesProvider, MinioProvider>();
        return services;
    }
    
    private static IServiceCollection AddDbContexts(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<WriteDbContext>(_ =>
            new WriteDbContext(configuration.GetConnectionString(InfrastructureConstants.DATABASE)!));
        
        services.AddScoped<IReadDbContext, ReadDbContext>(_ =>
            new ReadDbContext(configuration.GetConnectionString(InfrastructureConstants.DATABASE)!));
        return services;
    }
    
    private static IServiceCollection AddRepositories(
        this IServiceCollection services)
    {
        services.AddScoped<IVolunteersRepository, VolunteerRepository>();
        return services;
    }
    
    private static IServiceCollection AddTransactionManagement(
        this IServiceCollection services)
    {
        services.AddKeyedScoped<IUnitOfWork, UnitOfWork>("volunteer");
        services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
        
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        
        return services;
    }
    
    private static IServiceCollection AddHostedServices(
        this IServiceCollection services)
    {
        services.AddHostedService<FilesCleanerBackgroundService>();
        return services;
    }
    
    private static IServiceCollection AddMessageQueues(
        this IServiceCollection services)
    {
        services.AddSingleton<IMessageQueue<IEnumerable<FileInfo>>, InMemoryMessageQueue<IEnumerable<FileInfo>>>();
        return services;
    }
    
    private static IServiceCollection AddServices(
        this IServiceCollection services)
    {
        services.AddScoped<IFilesCleanerService, FilesCleanerService>();
        return services;
    }
}