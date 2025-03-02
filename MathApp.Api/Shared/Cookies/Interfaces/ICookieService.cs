using MathAppApi.Shared.Cookies;

namespace MathAppApi.Features.Authentication.Services.Interfaces;

public interface ICookieService
{
    public void SetCookie(HttpResponse response, string cookieKey, string value);
    public string? GetCookie(HttpRequest request, string cookieName);
    public void DeleteCookie(HttpResponse response, string cookieKey);
}