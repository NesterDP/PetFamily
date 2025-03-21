using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using PetFamily.Accounts.Infrastructure.DbContexts;
using PetFamily.Accounts.Infrastructure.Seeding;
using PetFamily.Core.Options;
using PetFamily.Volunteers.Application;
using PetFamily.Volunteers.Infrastructure.DbContexts;
using Respawn;
using Testcontainers.PostgreSql;
using PetFamily.Web;
using SpeciesWriteDbContext = PetFamily.Species.Infrastructure.DbContexts.WriteDbContext;
using SpeciesReadDbContext = PetFamily.Species.Infrastructure.DbContexts.ReadDbContext;
using SpeciesIReadDbContext = PetFamily.Species.Application.IReadDbContext;

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
        LoadSettingFromFiles(builder);
    }
    
    protected virtual void ConfigureDefaultServices(IServiceCollection services)
    {
        // Volunteers
        ReconfigureVolunteersServices(services);

        // Species
        ReconfigureSpeciesServices(services);

        // Accounts
        ReconfigureAccountsServices(services);
    }

    private void ReconfigureAccountsServices(IServiceCollection services)
    {
        var accountsDbContext = services.SingleOrDefault(s =>
            s.ServiceType == typeof(AccountsDbContext));

        if (accountsDbContext is not null)
            services.Remove(accountsDbContext);

        services.AddScoped<AccountsDbContext>(_ =>
            new AccountsDbContext(_dbContainer.GetConnectionString()));
    }

    private void ReconfigureSpeciesServices(IServiceCollection services)
    {
        var writeContext = services.SingleOrDefault(s =>
            s.ServiceType == typeof(SpeciesWriteDbContext));

        var readContext = services.SingleOrDefault(s =>
            s.ServiceType == typeof(SpeciesIReadDbContext));

        if (writeContext is not null)
            services.Remove(writeContext);

        if (readContext is not null)
            services.Remove(readContext);

        services.AddScoped<SpeciesWriteDbContext>(_ =>
            new SpeciesWriteDbContext(_dbContainer.GetConnectionString()));

        services.AddScoped<SpeciesIReadDbContext, SpeciesReadDbContext>(_ =>
            new SpeciesReadDbContext(_dbContainer.GetConnectionString()));
    }

    private void ReconfigureVolunteersServices(IServiceCollection services)
    {
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
    }
    
    private static void LoadSettingFromFiles(IWebHostBuilder builder)
    {
        var workingDirectory = Environment.CurrentDirectory;
        var projectRootDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
        var configPath = Path.Combine(projectRootDirectory, @"etc\testsettings.json");
        // DotNetEnv.Env.Load(configPath);
        
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddJsonFile(configPath);
        });
        
        builder.ConfigureServices(services =>
        {

            var configuration = services.BuildServiceProvider().GetService<IConfiguration>();
            services.Configure<AdminOptions>(configuration.GetSection("Admin"));
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        using var scope = Services.CreateScope();

        // Применяем миграции для VolunteerDbContext
        var volunteerDbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
        await volunteerDbContext.Database.MigrateAsync();

        // Применяем миграции для SpeciesDbContext
        var speciesDbContext =
            scope.ServiceProvider.GetRequiredService<SpeciesWriteDbContext>();
        await speciesDbContext.Database.MigrateAsync();

        // Применяем миграции для Accounts
        var accountsDbContext = scope.ServiceProvider
            .GetRequiredService<AccountsDbContext>();
        await accountsDbContext.Database.MigrateAsync();

        // Сидируем аккаунты
        var accountSeeder = scope.ServiceProvider.GetRequiredService<AccountsSeeder>();
        await accountSeeder.SeedAsync();

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
                //TablesToInclude = ["pets", "volunteers", "breeds", "species"]
                //SchemasToInclude = ["volunteers", "species", "accounts"]
            }
        );
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);

        using var scope = Services.CreateScope();
        var accountSeeder = scope.ServiceProvider.GetRequiredService<AccountsSeeder>();
        await accountSeeder.SeedAsync();
    }
}