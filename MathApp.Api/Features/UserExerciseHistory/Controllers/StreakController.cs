using MathApp.Dal.Interfaces;
using MathAppApi.Features.Authentication.Dtos;
using MathAppApi.Features.Authentication.Services.Interfaces;
using MathAppApi.Features.UserExerciseHistory.Dtos;
using MathAppApi.Shared.Utils;
using MathAppApi.Shared.Utils.Interfaces;
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

    private readonly IHistoryUtils _utils;

    public StreakController(IUserProfileRepo userProfileRepo, ILogger<StreakController> logger, IHistoryUtils utils)
    {
        _userProfileRepo = userProfileRepo;
        _logger = logger;
        _utils = utils;
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<StreakResponse>(StatusCodes.Status200OK)]
    [HttpGet("longest")]
    public async Task<IActionResult> GetLongest()
    {
        var userId = User.FindFirst("sub")?.Value;
        if (userId == null)
        {
            _logger.LogInformation("Longest streak fetch attempt with no userId.");
            return Unauthorized();
        }

        var userProfile = await _userProfileRepo.FindOneAsync(u => u.Id == userId);
        if (userProfile == null)
        {
            _logger.LogWarning("User not found during longest streak fetch attempt.");
            return BadRequest(new MessageResponse("User not found"));
        }

        StreakResponse response = await _utils.GetLongestStreak(userProfile);

        return Ok(response);
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<StreakResponse>(StatusCodes.Status200OK)]
    [HttpGet("current")]
    public async Task<IActionResult> GetCurrent()
    {
        var userId = User.FindFirst("sub")?.Value;
        if (userId == null)
        {
            _logger.LogInformation("Current streak fetch attempt with no userId.");
            return Unauthorized();
        }

        var userProfile = await _userProfileRepo.FindOneAsync(u => u.Id == userId);
        if (userProfile == null)
        {
            _logger.LogWarning("User not found during current streak fetch attempt.");
            return BadRequest(new MessageResponse("User not found"));
        }

        StreakResponse response = await _utils.GetCurrentStreak(userProfile);

        return Ok(response);
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<List<StreakResponse>>(StatusCodes.Status200OK)]
    [HttpGet("callendar")]
    public async Task<IActionResult> GetCallendar()
    {
        var userId = User.FindFirst("sub")?.Value;
        if (userId == null)
        {
            _logger.LogInformation("All streaks fetch attempt with no userId.");
            return Unauthorized();
        }

        var userProfile = await _userProfileRepo.FindOneAsync(u => u.Id == userId);
        if (userProfile == null)
        {
            _logger.LogWarning("User not found during all streaks fetch attempt.");
            return BadRequest(new MessageResponse("User not found"));
        }

        List<StreakResponse> response = await _utils.GetStreakCallendar(userProfile);

        return Ok(response);
    }
}
