using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.Discussions.Application.Abstractions;
using PetFamily.Discussions.Infrastructure.DbContexts;
using PetFamily.Discussions.Infrastructure.Repositories;
using PetFamily.Discussions.Infrastructure.TransactionServices;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.Structs;

namespace PetFamily.Discussions.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddDiscussionsInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDbContexts(configuration)
            .AddTransactionManagement()
            .AddRepositories();
        return services;
    }

    private static IServiceCollection AddDbContexts(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<WriteDbContext>(
            _ =>
                new WriteDbContext(configuration.GetConnectionString(InfrastructureConstants.DATABASE)!));

        services.AddScoped<IReadDbContext, ReadDbContext>(
            _ =>
                new ReadDbContext(configuration.GetConnectionString(InfrastructureConstants.DATABASE)!));

        return services;
    }

    private static IServiceCollection AddTransactionManagement(this IServiceCollection services)
    {
        services.AddKeyedScoped<IUnitOfWork, UnitOfWork>(UnitOfWorkSelector.Discussions);
        services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IDiscussionsRepository, DiscussionsRepository>();
        return services;
    }
}