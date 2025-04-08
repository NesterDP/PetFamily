using Amazon.S3;
using FileService.Infrastructure.Options;

namespace FileService.ApplicationConfiguration;

public static class S3StorageConfigurator
{
    public static IServiceCollection ConfigureS3Storage(this IServiceCollection services, IConfiguration configuration)
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
}