using MongoDB.Driver;

namespace FileService.ApplicationConfiguration;

public static class MongodbConfigurator
{
    public static IServiceCollection ConfigureMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IMongoClient>(new MongoClient(configuration.GetConnectionString("MongoConnection")));
        return services;
    }
}