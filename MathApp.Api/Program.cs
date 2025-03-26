using Dal.Extensions;
using MathAppApi.Features.Authentication.DataStorages;
using MathAppApi.Features.Authentication.Extensions;
using MathAppApi.Shared.Cookies.Extensions;
using MathAppApi.Shared.Emails;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using MathAppApi.Shared.Extensions;
using Models;
using MathAppApi.Features.UserExerciseHistory.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSqLiteDb();
builder.Services.AddRepos();
builder.Services.AddReposShared();
builder.Services.AddHistoryServices();
builder.Services.AddAuthenticationServices();
builder.Services.AddCookieService();
builder.Services.AddPasswordResetDataStorage();
builder.Services.UseFakeEmailService();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.ConfigureCookies();
builder.ConfigureJwt();
builder.Services.AddAuthorization();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "MathAppApi", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            []
        }
    });
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();
app.Run();