using FileService.API.Endpoints;
using FileService.ApplicationConfiguration;
using FileService.Middlewares;
using Hangfire;
using Serilog;

DotNetEnv.Env.Load("etc/.env");

var builder = WebApplication.CreateBuilder(args);

CultureConfigurator.Configure();
LoggerConfigurator.Configure(builder.Configuration);
builder.Services.ConfigureSwagger();
builder.Services.AddSerilog();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpoints();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors();

builder.Services.ConfigureHangfire(builder.Configuration);

builder.Services.ConfigureHttpClients();
builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.ConfigureAuthorization();
builder.Services.ConfigureUserData();

var app = builder.Build();

app.UseExceptionMiddleware();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHangfireDashboard();

app.UseCors(c => c.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.MapEndpoints();

app.Run();