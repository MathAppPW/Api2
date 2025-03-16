using Dal;
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
public class HistoryController : ControllerBase
{
    private readonly IUserProfileRepo _userProfileRepo;
    private readonly IUserHistoryEntryRepo _UserHistoryEntryRepo;

    private readonly ILogger<HistoryController> _logger;

    private readonly HistoryUtils utils;

    public HistoryController(IUserProfileRepo userProfileRepo, IUserHistoryEntryRepo userHistoryEntryRepo, ILogger<HistoryController> logger)
    {
        _userProfileRepo = userProfileRepo;
        _UserHistoryEntryRepo = userHistoryEntryRepo;
        _logger = logger;
        utils = new HistoryUtils(userHistoryEntryRepo);
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] HistoryEntryDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            _logger.LogInformation("User history entry post attempt with no userId.");
            return Unauthorized();
        }

        var userProfile = await _userProfileRepo.FindOneAsync(u => u.Id == userId);
        if (userProfile == null)
        {
            _logger.LogInformation("User not found during user history entry post attempt.");
            return BadRequest(new MessageResponse("User not found"));
        }

        UserHistoryEntry historyEntry = dto.ToModel();
        userProfile.History.Add(historyEntry.Id);

        await _UserHistoryEntryRepo.AddAsync(historyEntry);
        await _userProfileRepo.UpdateAsync(userProfile);

        return Ok();
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<UserHistoryEntry>(StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> Get([FromBody] string entryId)
    {
        var entry = await _UserHistoryEntryRepo.FindOneAsync(u => u.Id == entryId);
        if (entry == null)
        {
            _logger.LogInformation("User history entry not found during fetch attempt.");
            return BadRequest(new MessageResponse("User history entry not found"));
        }

        return Ok(entry);
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<List<string>>(StatusCodes.Status200OK)]
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            _logger.LogInformation("User history fetch attempt with no userId.");
            return Unauthorized();
        }

        var userProfile = await _userProfileRepo.FindOneAsync(u => u.Id == userId);
        if (userProfile == null)
        {
            _logger.LogInformation("User not found during user history fetch attempt.");
            return BadRequest(new MessageResponse("User not found"));
        }

        List<string> history = [];
        foreach (string entryId in userProfile.History)
        {
            var entry = await _UserHistoryEntryRepo.FindOneAsync(u => u.Id == entryId);
            if (entry == null)
            {
                _logger.LogInformation("User history entry not found during fetch attempt.");
                return BadRequest(new MessageResponse("Invalid user history entry found"));
            }
            history.Add(entryId);
        }

        return Ok(history);
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<List<TimeSpentDto>>(StatusCodes.Status200OK)]
    [HttpGet("time-spent")]
    public async Task<IActionResult> GetTimeSpent()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            _logger.LogInformation("User time spent fetch attempt with no userId.");
            return Unauthorized();
        }

        var userProfile = await _userProfileRepo.FindOneAsync(u => u.Id == userId);
        if (userProfile == null)
        {
            _logger.LogInformation("User not found during user time spent fetch attempt.");
            return BadRequest(new MessageResponse("User not found"));
        }

        List<TimeSpentDto> response = await utils.GetTimeSpentPerDay(userProfile);

        return Ok(response);
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<List<ExerciseCounterDto>>(StatusCodes.Status200OK)]
    [HttpGet("exercises-per-day")]
    public async Task<IActionResult> GetExerciseCountPerDay()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            _logger.LogInformation("Exercises count per day fetch attempt with no userId.");
            return Unauthorized();
        }

        var userProfile = await _userProfileRepo.FindOneAsync(u => u.Id == userId);
        if (userProfile == null)
        {
            _logger.LogInformation("User not found during exercises count per day fetch attempt.");
            return BadRequest(new MessageResponse("User not found"));
        }

        List<ExerciseCounterDto> response = await utils.GetExercisesCountPerDay(userProfile);

        return Ok(response);
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<int>(StatusCodes.Status200OK)]
    [HttpGet("exercises-count-all")]
    public async Task<IActionResult> GetExerciseCountAll()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            _logger.LogInformation("Exercises count all fetch attempt with no userId.");
            return Unauthorized();
        }

        var userProfile = await _userProfileRepo.FindOneAsync(u => u.Id == userId);
        if (userProfile == null)
        {
            _logger.LogInformation("User not found during exercises count all fetch attempt.");
            return BadRequest(new MessageResponse("User not found"));
        }

        int response = await utils.GetExercisesCountAll(userProfile);

        return Ok(response);
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] string entryId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            _logger.LogInformation("User history delete attempt with no userId.");
            return Unauthorized();
        }

        var userProfile = await _userProfileRepo.FindOneAsync(u => u.Id == userId);
        if (userProfile == null)
        {
            _logger.LogInformation("User not found during user history delete attempt.");
            return BadRequest(new MessageResponse("User not found"));
        }

        var entry = await _UserHistoryEntryRepo.FindOneAsync(u => u.Id == entryId);
        if (entry == null)
        {
            _logger.LogInformation("User history entry not found during delete attempt.");
            return BadRequest(new MessageResponse("User history entry not found"));
        }

        userProfile.History.Remove(entryId);

        await _UserHistoryEntryRepo.RemoveAsync(entry);
        await _userProfileRepo.UpdateAsync(userProfile);

        return Ok();
    }


}
