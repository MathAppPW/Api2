using MathApp.Dal.Interfaces;
using MathAppApi.Features.Authentication.Dtos;
using MathAppApi.Features.Authentication.Services.Interfaces;
using MathAppApi.Features.UserProfile.Dtos;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace MathAppApi.Features.UserProfile.Controllers;

[ApiController]
[Route("[controller]")]
public class ProfileSkinController : ControllerBase
{
    private readonly int skinCount = 10;
    private readonly IUserRepo _userRepo;
    private readonly ICookieService _cookieService;
    private readonly ITokenService _tokenService;

    private readonly ILogger<ProfileSkinController> _logger;

    public ProfileSkinController(IUserRepo userRepo, ICookieService cookieService, ITokenService tokenService, ILogger<ProfileSkinController> logger)
    {
        _userRepo = userRepo;
        _cookieService = cookieService;
        _tokenService = tokenService;
        _logger = logger;
    }

    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] SkinDto dto)
    {
        if(dto.SkinId < 0 || dto.SkinId >= skinCount)
        {
            return BadRequest(new MessageResponse("Invalid skin id"));
        }

        var validationResponse = await ValidateRefreshToken();
        if (validationResponse is not OkResult)
        {
            return validationResponse;
        }

        var user = await _userRepo.FindOneAsync(u => u.Id == dto.UserId);
        if (user == null)
        {
            return BadRequest(new MessageResponse("User not found"));
        }

        user.ProfileSkin = dto.SkinId;

        return Ok();
    }

    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> Get([FromBody] BasicDto dto)
    {
        var validationResponse = await ValidateRefreshToken();
        if (validationResponse is not OkResult)
        {
            return validationResponse;
        }

        var user = await _userRepo.FindOneAsync(u => u.Id == dto.UserId);
        if (user == null)
        {
            return BadRequest(new MessageResponse("User not found"));
        }

        return Ok(new { user.ProfileSkin });
    }

    public async Task<IActionResult> ValidateRefreshToken()
    {
        var refreshToken = _cookieService.GetCookie(Request, "RefreshToken");
        if (refreshToken == null)
        {
            _logger.LogInformation("Update attempt without refresh token.");
            return Unauthorized();
        }
        var isTokenValid = await _tokenService.IsRefreshTokenValid(refreshToken);
        if (!isTokenValid)
        {
            _logger.LogInformation("Update attempt with invalid refresh token.");
            return Unauthorized();
        }
        return Ok();
    }

}
