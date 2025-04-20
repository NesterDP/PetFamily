using NotificationService.API.Endpoints;
using NotificationService.ApplicationConfiguration;
using NotificationService.Middlewares;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

CultureConfigurator.Configure();
LoggerConfigurator.Configure(builder.Configuration);
builder.Services.AddSwaggerGen();
builder.Services.AddSerilog();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpoints();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors();

var app = builder.Build();

app.UseExceptionMiddleware();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(c => c.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.MapEndpoints();

app.Run();