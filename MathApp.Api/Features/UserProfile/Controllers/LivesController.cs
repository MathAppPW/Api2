using MathApp.Dal.Interfaces;
using MathAppApi.Features.Authentication.Dtos;
using MathAppApi.Features.Authentication.Services.Interfaces;
using MathAppApi.Features.UserProfile.Dtos;
using MathAppApi.Features.UserProfile.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Security.Claims;

namespace MathAppApi.Features.UserProfile.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class LivesController : ControllerBase
{
    public static readonly int _livesUpdateInterval = 5;
    public static readonly int _maxLives = 5;
    private readonly IUserProfileRepo _userProfileRepo;

    private readonly ILivesService _livesService;

    private readonly ILogger<LivesController> _logger;

    public LivesController(IUserProfileRepo userProfileRepo, ILivesService livesService, ILogger<LivesController> logger)
    {
        _userProfileRepo = userProfileRepo;
        _livesService = livesService;
        _logger = logger;
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<LivesResponse>(StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> UpdateLives()
    {
        var userId = User.FindFirst("sub")?.Value;
        if (userId == null)
        {
            _logger.LogInformation("Lives update attempt with no userId.");
            return Unauthorized();
        }

        var userProfile = await _userProfileRepo.FindOneAsync(u => u.Id == userId);
        if (userProfile == null)
        {
            _logger.LogInformation("User not found during lives update attempt.");
            return BadRequest(new MessageResponse("User not found"));
        }

        await _livesService.UpdateLives(userProfile);

        return Ok(new LivesResponse
        {
            Lives = userProfile.Lives,
            SecondsToHeal = await _livesService.GetSecondsToHeal(userProfile)
        });
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<LivesResponse>(StatusCodes.Status200OK)]
    [HttpPost("damage")]
    public async Task<IActionResult> Damage()
    {
        var userId = User.FindFirst("sub")?.Value;
        if (userId == null)
        {
            _logger.LogInformation("Lives damage attempt with no userId.");
            return Unauthorized();
        }

        var userProfile = await _userProfileRepo.FindOneAsync(u => u.Id == userId);
        if (userProfile == null)
        {
            _logger.LogInformation("User not found during lives damage attempt.");
            return BadRequest(new MessageResponse("User not found"));
        }

        await UpdateLives();
        userProfile.Lives = Math.Max(userProfile.Lives - 1, 0);

        await _userProfileRepo.UpdateAsync(userProfile);

        return Ok(new LivesResponse
        {
            Lives = userProfile.Lives,
            SecondsToHeal = await _livesService.GetSecondsToHeal(userProfile)
        });
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<LivesResponse>(StatusCodes.Status200OK)]
    [HttpPost("heal")]
    public async Task<IActionResult> Heal()
    {
        var userId = User.FindFirst("sub")?.Value;
        if (userId == null)
        {
            _logger.LogInformation("Lives heal attempt with no userId.");
            return Unauthorized();
        }

        var userProfile = await _userProfileRepo.FindOneAsync(u => u.Id == userId);
        if (userProfile == null)
        {
            _logger.LogInformation("User not found during lives heal attempt.");
            return BadRequest(new MessageResponse("User not found"));
        }

        await UpdateLives();
        userProfile.Lives = Math.Min(userProfile.Lives + 1, _maxLives);
        if (userProfile.Lives == _maxLives)
        {
            userProfile.LastLivesUpdate = DateTime.UtcNow;
        }

        await _userProfileRepo.UpdateAsync(userProfile);

        return Ok(new LivesResponse
        {
            Lives = userProfile.Lives,
            SecondsToHeal = await _livesService.GetSecondsToHeal(userProfile)
        });
    }
}
