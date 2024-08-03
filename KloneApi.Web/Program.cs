using FluentValidation;
using KloneApi.Identity.Command;
using KloneApi.Identity.Controllers;
using KloneApi.SharedDomain.Configs;
using KloneApi.SharedDomain.Middlewares;

using KloneApi.SharedDomain.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Enums;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = builder.Configuration;

#region setup configs
builder.Services.Configure<DatabaseConfig>(configuration.GetSection(DatabaseConfig.Position));
#endregion

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(OnboardingController).Assembly));
builder.Services.AddDbContext<KloneDbContext>(options =>
    options.UseSqlServer("Server=localhost,1433;Database=KloneDb;User Id=sa;Password=Password123;TrustServerCertificate=True"));
 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

//Add Validators
builder.Services.AddValidatorsFromAssemblyContaining<SignUp.SignUpRequestValidator>(); //Identity
builder.Services.AddFluentValidationAutoValidation((configuration) =>
{
    configuration.ValidationStrategy = ValidationStrategy.Annotations;
    configuration.OverrideDefaultResultFactoryWith<ValidationResultMiddleware>();
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<ResponseWrapperMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.MapControllers();

app.Run();
