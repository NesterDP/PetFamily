using System.Globalization;
using PetFamily.Accounts.Application;
using PetFamily.Accounts.Infrastructure;
using PetFamily.Accounts.Presentation;
using PetFamily.Web.Middlewares;
using PetFamily.Application;
using PetFamily.Species.Application;
using PetFamily.Species.Infrastructure;
using PetFamily.Species.Presentation;
using PetFamily.Species.Presentation.Species;
using PetFamily.Volunteers.Application;
using PetFamily.Volunteers.Infrastructure;
using PetFamily.Volunteers.Presentation.Volunteers;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

var cultureInfo = new CultureInfo("ru-RU"); // Используем культуру, где точка — разделитель
cultureInfo.NumberFormat.NumberDecimalSeparator = "."; // Устанавливаем разделитель
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;


/*builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = actionContext =>
    {
        var responseError = new ResponseError(
            "model.binding.error", "failed to bind the received model", "null");
        return new BadRequestObjectResult(Envelope.Error([responseError]));
    };
});*/

Log.Logger = new LoggerConfiguration()
    .WriteTo.Seq(builder.Configuration.GetConnectionString("Seq")
                 ?? throw new ArgumentNullException("Seq"))
    .WriteTo.Console()
    .Enrich.WithMachineName()
    .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
    .CreateLogger();


builder.Services.AddSwaggerGen();
builder.Services.AddSerilog();

// настройка сервисов, связанных с авторизацией
// ..

// настройка модулей
builder.Services
    .AddAccountsApplication()
    .AddAccountsInfrastructure(builder.Configuration)

    .AddVolunteersApplication()
    .AddVolunteersInfrastructure(builder.Configuration)

    .AddSpeciesApplication()
    .AddSpeciesContracts()
    .AddSpeciesInfrastructure(builder.Configuration);


builder.Services.AddControllers()
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

//app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();

namespace PetFamily.Web
{
    public partial class Program; // для доступа к этому классу в другом проекте (интеграционные тесты)
}