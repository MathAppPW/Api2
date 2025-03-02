using Models;

namespace MathAppApi.Features.Authentication.Services.Interfaces;

public interface ITokenService
{
    public Task<string> GetRefreshToken(User user);
    public Task<string?> GetAccessToken(string refreshToken);
    public Task<bool> IsRefreshTokenValid(string token);
    public Task RemoveRefreshToken(string token);
}