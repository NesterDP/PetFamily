using Amazon.S3;
using FileService.Infrastructure.MongoDataAccess;
using FileService.Infrastructure.Options;
using FileService.Infrastructure.Providers;
using FileService.Infrastructure.Repositories;
using MongoDB.Driver;

namespace FileService.ApplicationConfiguration;

public static class InfrastructureConfigurator
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddScoped<FileMongoDbContext>()
            .AddScoped<IFileRepository, FileRepository>()
            .AddScoped<IFileProvider, FileProvider>()
            .ConfigureS3Storage(configuration)
            .ConfigureMongoDb(configuration);
        
        return services;
    }
    
    private static IServiceCollection ConfigureS3Storage(this IServiceCollection services, IConfiguration configuration)
    {
        var s3StorageOptions = configuration.GetSection(MinioOptions.MINIO).Get<MinioOptions>()
                               ?? throw new ApplicationException("Missing minio configuration");

        services.AddSingleton<IAmazonS3>(_ =>
        {
            var config = new AmazonS3Config
            {
                ServiceURL = s3StorageOptions.Endpoint,
                ForcePathStyle = true,
                UseHttp = true
            };

            return new AmazonS3Client(s3StorageOptions.AccessKey, s3StorageOptions.SecretKey, config);
        });
        
        return services;
    }
    
    private static IServiceCollection ConfigureMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IMongoClient>(new MongoClient(configuration.GetConnectionString("MongoConnection")));
        return services;
    }
}