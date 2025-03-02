using MathAppApi.Features.Authentication.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace MathAppApi.Shared.Cookies;

public class CookieService : ICookieService
{
    private readonly Dictionary<string, CookieConfig> _cookieConfigs;
    public CookieConfig DefaultConfig { get; set; }

    public CookieService(IOptions<CookieSettings> options)
    {
        _cookieConfigs = options.Value.CookieConfigs;
        DefaultConfig = new CookieConfig();
    }
    
    public void SetCookie(HttpResponse response, string cookieKey, string value)
    {
        _cookieConfigs.TryGetValue(cookieKey, out var cookieConfig);
        cookieConfig ??= DefaultConfig;
        var cookieOptions = GetCookieOptions(cookieConfig, response.HttpContext);
        response.Cookies.Append(cookieKey, value, cookieOptions);
    }

    private CookieOptions GetCookieOptions(CookieConfig cookieConfig, HttpContext context)
    {
        var builder = new CookieBuilder();
        builder.Name = cookieConfig.Name;
        builder.Domain = cookieConfig.Domain;
        builder.Path = cookieConfig.Path;
        builder.SecurePolicy = cookieConfig.Secure ? CookieSecurePolicy.Always : CookieSecurePolicy.None;
        builder.HttpOnly = cookieConfig.HttpOnly;
        builder.SameSite = Enum.Parse<SameSiteMode>(cookieConfig.SameSite);
        builder.Expiration = TimeSpan.FromMinutes(cookieConfig.ExpirationMinutes);
        return builder.Build(context);
    }
    
    public string? GetCookie(HttpRequest request, string cookieName)
    {
        var result = request.Cookies[cookieName];
        return result;
    }

    public void DeleteCookie(HttpResponse response, string cookieKey)
    {
        response.Cookies.Delete(cookieKey);
    }
}