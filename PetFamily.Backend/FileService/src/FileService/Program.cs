using FileService.ApplicationConfiguration;
using FileService.Endpoints;
using FileService.Middlewares;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

CultureConfigurator.Configure();
LoggerConfigurator.Configure(builder.Configuration);
builder.Services.AddSwaggerGen();
builder.Services.AddSerilog();

builder.Services.ConfigureS3Storage(builder.Configuration);

builder.Services.AddEndpoints();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseExceptionMiddleware();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapEndpoints();

app.Run();
