using System.Text;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PetFamily.Accounts.Application;
using PetFamily.Accounts.Infrastructure;
using PetFamily.Accounts.Presentation;
using PetFamily.Web.Middlewares;
using PetFamily.Species.Application;
using PetFamily.Species.Infrastructure;
using PetFamily.Species.Presentation;
using PetFamily.Species.Presentation.Species;
using PetFamily.Volunteers.Application;
using PetFamily.Volunteers.Infrastructure;
using PetFamily.Volunteers.Presentation;
using PetFamily.Volunteers.Presentation.Volunteers;
using PetFamily.Web.ApplicationConfiguration;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

CultureConfigurator.Configure();

LoggerConfigurator.Configure(builder);

//builder.Services.AddSwaggerGen();
SwaggerConfigurator.ConfigureSwagger(builder);

builder.Services.AddSerilog();

// настройка сервисов, связанных с авторизацией


// настройка модулей
builder.Services
    .AddAccountsApplication()
    .AddAccountsInfrastructure(builder.Configuration)
    .AddAccountsContracts()

    .AddVolunteersApplication()
    .AddVolunteersInfrastructure(builder.Configuration)
    .AddVolunteersContracts()

    .AddSpeciesApplication()
    .AddSpeciesInfrastructure(builder.Configuration)
    .AddSpeciesContracts();


builder.Services.AddControllers()
    //.InterceptModelBindingError()
    .AddApplicationPart(typeof(AccountsController).Assembly)
    .AddApplicationPart(typeof(VolunteersController).Assembly)
    .AddApplicationPart(typeof(SpeciesController).Assembly);


builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();

app.UseExceptionMiddleware();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    //await app.ApplyMigrations();
}


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

namespace PetFamily.Web
{
    public partial class Program; // для доступа к этому классу в другом проекте (интеграционные тесты)
}