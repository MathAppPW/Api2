using MathApp.Dal.Interfaces;
using MathAppApi.Features.Authentication.Dtos;
using MathAppApi.Features.Authentication.Services.Interfaces;
using MathAppApi.Features.UserProfile.Dtos;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace MathAppApi.Features.UserProfile.Controllers;

[ApiController]
[Route("[controller]")]
public class LivesController : ControllerBase
{
    private readonly int _livesUpdateInterval = 5;
    private readonly int _maxLives = 10;
    private readonly IUserRepo _userRepo;
    private readonly ICookieService _cookieService;
    private readonly ITokenService _tokenService;

    private readonly ILogger<LivesController> _logger;

    public LivesController(IUserRepo userRepo, ICookieService cookieService, ITokenService tokenService, ILogger<LivesController> logger)
    {
        _userRepo = userRepo;
        _cookieService = cookieService;
        _tokenService = tokenService;
        _logger = logger;
    }

    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
    [HttpPost("update")]
    public async Task<IActionResult> UpdateLives([FromBody] BasicDto dto)
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

        var currentTime = DateTime.UtcNow;
        var lastUpdate = user.LastLivesUpdate;
        var timeDifference = currentTime - lastUpdate;
        var livesToAdd = (int)timeDifference.TotalMinutes / _livesUpdateInterval;

        user.Lives = Math.Min(user.Lives + livesToAdd, _maxLives);
        if(user.Lives == _maxLives)
        {
            user.LastLivesUpdate = currentTime;
        }
        else
        {
            user.LastLivesUpdate = lastUpdate.AddMinutes(livesToAdd * _livesUpdateInterval);
        }

        return Ok(user.Lives);
    }

    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
    [HttpPost("damage")]
    public async Task<IActionResult> Damage([FromBody] BasicDto dto)
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

        await UpdateLives(dto);
        user.Lives = Math.Max(user.Lives - 1, 0);

        return Ok(user.Lives);
    }

    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
    [HttpPost("heal")]
    public async Task<IActionResult> Heal([FromBody] BasicDto dto)
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

        await UpdateLives(dto);
        user.Lives = Math.Min(user.Lives + 1, _maxLives);
        if (user.Lives == _maxLives)
        {
            user.LastLivesUpdate = DateTime.UtcNow;
        }

        return Ok(user.Lives);
    }

    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
    [HttpGet("time-to-heal")]
    public async Task<IActionResult> TimeToHeal([FromBody] BasicDto dto)
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

        await UpdateLives(dto);
        if(user.Lives == _maxLives)
        {
            return Ok(0);
        }
        else
        {
            int timeFromLastUpdate = (int)(DateTime.UtcNow - user.LastLivesUpdate).TotalSeconds;
            int secondsToHeal = _livesUpdateInterval * 60 - timeFromLastUpdate;
            return Ok(new { secondsToHeal });
        }
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
