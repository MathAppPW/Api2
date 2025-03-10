using MathApp.Dal.Interfaces;
using MathAppApi.Features.Authentication.Dtos;
using MathAppApi.Features.Authentication.Services.Interfaces;
using MathAppApi.Features.UserProfile.Dtos;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace MathAppApi.Features.UserProfile.Controllers;

[ApiController]
[Route("[controller]")]
public class ExperienceController : ControllerBase
{
    private readonly IUserRepo _userRepo;
    private readonly ICookieService _cookieService;
    private readonly ITokenService _tokenService;

    private readonly ILogger<ExperienceController> _logger;

    public ExperienceController(IUserRepo userRepo, ICookieService cookieService, ITokenService tokenService, ILogger<ExperienceController> logger)
    {
        _userRepo = userRepo;
        _cookieService = cookieService;
        _tokenService = tokenService;
        _logger = logger;
    }

    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] ExperienceDto dto)
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

        user.Experience += dto.Amount;
        var leveledUp = false;
        if(user.Experience / 1000 > user.Level)
        {
            user.Level = user.Experience / 1000;
            leveledUp = true;
        }

        return Ok(new { leveledUp, user.Level, user.Experience });
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

        return Ok(new { user.Level, user.Experience });
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
