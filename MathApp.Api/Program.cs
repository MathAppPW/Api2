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
using MathAppApi.Features.Exercise.Extensions;
using MathAppApi.Features.UserProgress.Extensions;
using MathAppApi.Features.Leaderboard.Extensions;
using MathAppApi.Features.Rankings.Extensions;
using MathAppApi.Features.UserProfile.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

var dbEnv = Environment.GetEnvironmentVariable("DB_ENVIRONMENT");
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
builder.Services.AddDbFromEnvironment(dbEnv, connectionString);

builder.Services.AddRepos();
builder.Services.AddReposShared();
builder.Services.AddExerciseServices();
builder.Services.AddProgressServices();
builder.Services.AddLeaderboardServices();
builder.Services.AddAuthenticationServices();
builder.Services.AddRankings();
builder.Services.AddProfileServices();
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