using NotificationService.Core.Constants;
using NotificationService.Core.Structs;
using NotificationService.Infrastructure.DbContexts;
using NotificationService.Infrastructure.Repositories;
using NotificationService.Infrastructure.TransactionServices;

namespace NotificationService.ApplicationConfiguration;

public static class InfrastructureConfigurator
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDbContexts(configuration)
            .AddRepositories()
            .AddTransactionManagement();

        return services;
    }
    
    private static IServiceCollection AddDbContexts(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<WriteDbContext>(_ =>
            new WriteDbContext(configuration.GetConnectionString(InfrastructureConstants.DATABASE)!));
        
        return services;
    }

    private static IServiceCollection AddRepositories(
        this IServiceCollection services)
    {
        services.AddScoped<INotificationsRepository, NotificationsRepository>();
        return services;
    }

    private static IServiceCollection AddTransactionManagement(this IServiceCollection services)
    {
        services.AddKeyedScoped<IUnitOfWork, UnitOfWork>(UnitOfWorkSelector.UsersNotificationSettings);
        return services;
    }
}