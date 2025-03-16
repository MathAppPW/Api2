using MathApp.Dal.Interfaces;
using MathAppApi.Features.Authentication.Dtos;
using MathAppApi.Features.Authentication.Services.Interfaces;
using MathAppApi.Features.UserExerciseHistory.Dtos;
using MathAppApi.Features.UserExerciseHistory.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Security.Claims;

namespace MathAppApi.Features.UserExerciseHistory.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class StreakController : ControllerBase
{
    private readonly IUserProfileRepo _userProfileRepo;

    private readonly ILogger<StreakController> _logger;

    private readonly HistoryUtils utils;

    public StreakController(IUserProfileRepo userProfileRepo, IUserHistoryEntryRepo userHistoryEntryRepo, ILogger<StreakController> logger)
    {
        _userProfileRepo = userProfileRepo;
        _logger = logger;
        utils = new HistoryUtils(userHistoryEntryRepo);
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<StreakResponse>(StatusCodes.Status200OK)]
    [HttpGet("longest")]
    public async Task<IActionResult> GetLongest()
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

        StreakResponse response = await utils.GetLongestStreak(userProfile);

        return Ok(response);
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<StreakResponse>(StatusCodes.Status200OK)]
    [HttpGet("current")]
    public async Task<IActionResult> GetCurrent()
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

        StreakResponse response = await utils.GetCurrentStreak(userProfile);

        return Ok(response);
    }
}
