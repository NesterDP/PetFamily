using PetFamily.API.Valiadtion;
using PetFamily.Application;
using PetFamily.Application.Volunteers;
using PetFamily.Infrastructure;
using PetFamily.Application.Volunteers.CreateVolunteer;
using PetFamily.Infrastructure.Repositories;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddInfrastructure()
    .AddApplication();

// builder.Services.AddFluentValidationAutoValidation(configuration =>
// {
//     configuration.OverrideDefaultResultFactoryWith<CustomResultFactory>();
// });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
