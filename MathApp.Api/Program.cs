using Dal.Extensions;
using MathAppApi.Features.Authentication.Extensions;
using MathAppApi.Shared.Cookies.Extensions;
using Microsoft.AspNetCore.Identity;
using Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddSqLiteDb();
builder.Services.AddRepos();
builder.Services.AddAuthenticationServices();
builder.Services.AddCookieService();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.ConfigureCookies();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();
app.Run();
