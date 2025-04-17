using FileService.Communication;
using PetFamily.Web.Middlewares;
using PetFamily.Web.ApplicationConfiguration;
using Serilog;

DotNetEnv.Env.Load("etc/.env");

var builder = WebApplication.CreateBuilder(args);

CultureConfigurator.Configure();

LoggerConfigurator.Configure(builder.Configuration);

builder.Services.ConfigureSwagger();

builder.Services.AddSerilog();

builder.Services.AddModules(builder.Configuration);

builder.Services.AddConfiguredControllers();

builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.ConfigureAuthorization();
builder.Services.ConfigureUserData();

builder.Services.AddMessageBus(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddFileHttpCommunication(builder.Configuration);

var app = builder.Build();

app.ApplyMigrations();

await app.SeedAsync();

app.UseExceptionMiddleware();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
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