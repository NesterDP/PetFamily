using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using PetFamily.Volunteers.Application;
using PetFamily.Volunteers.Infrastructure.DbContexts;
using Respawn;
using Testcontainers.PostgreSql;
using PetFamily.Web;

namespace PetFamily.IntegrationTests;

public class IntegrationTestsWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private Respawner _respawner = default!;
    private DbConnection _dbConnection = default!;

    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres")
        .WithDatabase("pet_family_tests")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(ConfigureDefaultServices);
    }

    protected virtual void ConfigureDefaultServices(IServiceCollection services)
    {
        // Volunteers
        var writeContext = services.SingleOrDefault(s =>
            s.ServiceType == typeof(WriteDbContext));

        var readContext = services.SingleOrDefault(s =>
            s.ServiceType == typeof(IReadDbContext));

        if (writeContext is not null)
            services.Remove(writeContext);

        if (readContext is not null)
            services.Remove(readContext);
        
        services.AddScoped<WriteDbContext>(_ =>
            new WriteDbContext(_dbContainer.GetConnectionString()));

        services.AddScoped<IReadDbContext, ReadDbContext>(_ =>
            new ReadDbContext(_dbContainer.GetConnectionString()));

        // Species
        writeContext = services.SingleOrDefault(s =>
            s.ServiceType == typeof(Species.Infrastructure.DbContexts.WriteDbContext));

        readContext = services.SingleOrDefault(s =>
            s.ServiceType == typeof(Species.Application.IReadDbContext));

        if (writeContext is not null)
            services.Remove(writeContext);

        if (readContext is not null)
            services.Remove(readContext);
        
        services.AddScoped<Species.Infrastructure.DbContexts.WriteDbContext>(_ =>
            new Species.Infrastructure.DbContexts.WriteDbContext(_dbContainer.GetConnectionString()));

        services.AddScoped<Species.Application.IReadDbContext, Species.Infrastructure.DbContexts.ReadDbContext>(_ =>
            new Species.Infrastructure.DbContexts.ReadDbContext(_dbContainer.GetConnectionString()));
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        using var scope = Services.CreateScope();

        // Применяем миграции для VolunteerDbContext
        var volunteerDbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
        await volunteerDbContext.Database.MigrateAsync();

        // Применяем миграции для SpeciesDbContext
        var speciesDbContext = scope.ServiceProvider.GetRequiredService<Species.Infrastructure.DbContexts.WriteDbContext>();
        await speciesDbContext.Database.MigrateAsync();

        _dbConnection = new NpgsqlConnection(_dbContainer.GetConnectionString());
        await InitializeRespawner();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
    }

    private async Task InitializeRespawner()
    {
        await _dbConnection.OpenAsync();
        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                TablesToInclude = ["pets", "volunteers", "breeds", "species"]
                //SchemasToInclude = ["pets", "volunteers", "breeds", "species"]
            }
        );
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }
}