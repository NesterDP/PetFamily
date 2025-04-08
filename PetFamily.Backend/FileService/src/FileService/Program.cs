using FileService.ApplicationConfiguration;
using FileService.Endpoints;
using FileService.Infrastructure.MongoDataAccess;
using FileService.Infrastructure.Repositories;
using FileService.Middlewares;
using Hangfire;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

CultureConfigurator.Configure();
LoggerConfigurator.Configure(builder.Configuration);
builder.Services.AddSwaggerGen();
builder.Services.AddSerilog();

builder.Services.ConfigureS3Storage(builder.Configuration);
builder.Services.ConfigureMongoDb(builder.Configuration);
builder.Services.AddScoped<FileMongoDbContext>();
builder.Services.AddScoped<IFileRepository, FileRepository>();

builder.Services.AddEndpoints();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors();

builder.Services.ConfigureHangfire(builder.Configuration);

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
