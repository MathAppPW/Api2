using System.Text;
using MathAppApi.Features.Authentication.Services;
using MathAppApi.Features.Authentication.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace MathAppApi.Features.Authentication.Extensions;

public static class DependencyInjection
{
    public static void AddAuthenticationServices(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserDataValidator, UserDataValidator>();
    }

    public static void ConfigureJwt(this WebApplicationBuilder builder)
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        if (jwtSettings["Issuer"] is null || jwtSettings["Audience"] is null || jwtSettings["Secret"] is null)
        {
            throw new InvalidOperationException("One or more config parameters are null, check configuration!");
        }

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!))
                };
            });
    }
}