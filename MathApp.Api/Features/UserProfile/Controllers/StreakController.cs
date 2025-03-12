using MathApp.Dal.Interfaces;
using MathAppApi.Features.Authentication.Dtos;
using MathAppApi.Features.Authentication.Services.Interfaces;
using MathAppApi.Features.UserProfile.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Security.Claims;

namespace MathAppApi.Features.UserProfile.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class StreakController : ControllerBase
{
    private readonly IUserProfileRepo _userProfileRepo;

    private readonly ILogger<StreakController> _logger;

    public StreakController(IUserProfileRepo userProfileRepo, ILogger<StreakController> logger)
    {
        _userProfileRepo = userProfileRepo;
        _logger = logger;
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<StreakResponse>(StatusCodes.Status200OK)]
    [HttpPost("increase")]
    public async Task<IActionResult> Increase()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            _logger.LogInformation("Streak increase attempt with no userId.");
            return Unauthorized();
        }

        var userProfile = await _userProfileRepo.FindOneAsync(u => u.Id == userId);
        if (userProfile == null)
        {
            _logger.LogInformation("User not found during streak increase attempt.");
            return BadRequest(new MessageResponse("User not found"));
        }

        userProfile.Streak++;

        await _userProfileRepo.UpdateAsync(userProfile);

        return Ok(new StreakResponse { Streak = userProfile.Streak });
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPost("reset")]
    public async Task<IActionResult> Reset()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            _logger.LogInformation("Streak reset attempt with no userId.");
            return Unauthorized();
        }

        var userProfile = await _userProfileRepo.FindOneAsync(u => u.Id == userId);
        if (userProfile == null)
        {
            _logger.LogInformation("User not found during streak reset attempt.");
            return BadRequest(new MessageResponse("User not found"));
        }

        userProfile.Streak = 0;

        await _userProfileRepo.UpdateAsync(userProfile);

        return Ok();
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<StreakResponse>(StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            _logger.LogInformation("Streak fetch attempt with no userId.");
            return Unauthorized();
        }

        var userProfile = await _userProfileRepo.FindOneAsync(u => u.Id == userId);
        if (userProfile == null)
        {
            _logger.LogInformation("User not found during streak fetch attempt.");
            return BadRequest(new MessageResponse("User not found"));
        }

        return Ok(new StreakResponse { Streak = userProfile.Streak });
    }
}
