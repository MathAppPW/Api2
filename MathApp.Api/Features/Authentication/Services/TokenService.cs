using System.Security.Claims;
using System.Text;
using MathApp.Dal.Interfaces;
using MathAppApi.Features.Authentication.Services.Interfaces;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Models;

namespace MathAppApi.Features.Authentication.Services;

public class TokenService : ITokenService
{
    private readonly IUserRepo _userRepo;
    private readonly string _secret;
    private readonly string _issuer;
    private readonly string _audience;

    private readonly TimeSpan _refreshTokenTimeSpan;
    private readonly TimeSpan _accessTokenTimeSpan;

    public TokenService(IConfiguration configuration, IUserRepo userRepo)
    {
        _userRepo = userRepo;
        var jwtSettings = configuration.GetSection("JwtSettings");
        _secret = jwtSettings["Secret"] ?? throw new InvalidOperationException();
        _issuer = jwtSettings["Issuer"]!;
        _audience = jwtSettings["Audience"]!;
        _refreshTokenTimeSpan = TimeSpan.FromMinutes(int.Parse(jwtSettings["RefreshTimeSpan"]!));
        _accessTokenTimeSpan = TimeSpan.FromMinutes(int.Parse(jwtSettings["AccessTimeSpan"]!));
    }
    
    public async Task<string> GetRefreshToken(User user)
    {
        var expirationDateTime = DateTime.UtcNow.Add(_refreshTokenTimeSpan);
        var newVersion = Guid.NewGuid().ToString("N");
        user.TokenVersion = newVersion;
        await _userRepo.UpdateAsync(user);
        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Sub, user.Id),
            new (JwtRegisteredClaimNames.Jti, newVersion), //N specifies raw not separated format
        };
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expirationDateTime,
            SigningCredentials = GetSigningCredentials(),
        };
        var tokenHandler = new JsonWebTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return token;
    }

    public async Task<string?> GetAccessToken(string refreshToken)
    {
        var id = GetUserId(refreshToken);
        var user = await _userRepo.GetAsync(id);
        if (user == null)
            return null;
        return GetAccessToken(user);
    }

    public async Task<bool> IsRefreshTokenValid(string token)
    {
        var jwt = new JsonWebToken(token);
        var id = jwt.Subject;
        var user = await _userRepo.GetAsync(id);
        if (user is null)
            return false;
        var exp = jwt.ValidTo;
        return exp > DateTime.Now && user.TokenVersion == jwt.Id;
    }

    private string GetAccessToken(User user)
    {
        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Sub, user.Id),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
        };

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(_accessTokenTimeSpan),
            Audience = _audience,
            Issuer = _issuer,
            IssuedAt = DateTime.UtcNow,
        };
        var tokenHandler = new JsonWebTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return token;
    }

    private string GetUserId(string token)
    {
        var jwt = new JsonWebToken(token);
        var userId = jwt.Subject;
        return userId;
    }

    private SigningCredentials GetSigningCredentials()
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        return credentials;
    }
    
    public async Task RemoveRefreshToken(string token)
    {
        var userId = GetUserId(token);
        var user = await _userRepo.GetAsync(userId);
        if (user == null)
            return;
        user.TokenVersion = null;
    }
}