using Dal;
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
public class ExperienceController : ControllerBase
{
    private readonly IUserProfileRepo _userProfileRepo;

    private readonly ILogger<ExperienceController> _logger;

    public ExperienceController(IUserProfileRepo userProfileRepo, ILogger<ExperienceController> logger)
    {
        _userProfileRepo = userProfileRepo;
        _logger = logger;
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ExperienceResponse>(StatusCodes.Status200OK)]
    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] ExperienceDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            _logger.LogInformation("Experience increase attempt with no userId.");
            return Unauthorized();
        }

        var userProfile = await _userProfileRepo.FindOneAsync(u => u.Id == userId);
        if (userProfile == null)
        {
            _logger.LogInformation("User not found during experience increase attempt.");
            return BadRequest(new MessageResponse("User not found"));
        }

        userProfile.Experience += dto.Amount;
        var leveledUp = false;
        if(userProfile.Experience / 1000 > userProfile.Level)
        {
            userProfile.Level = userProfile.Experience / 1000;
            leveledUp = true;
        }

        await _userProfileRepo.UpdateAsync(userProfile);

        return Ok(new ExperienceResponse {
            LeveledUp = leveledUp,
            Level = userProfile.Level,
            Experience = userProfile.Experience });
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ExperienceResponse>(StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            _logger.LogInformation("Experience fetch attempt with no userId.");
            return Unauthorized();
        }

        var userProfile = await _userProfileRepo.FindOneAsync(u => u.Id == userId);
        if (userProfile == null)
        {
            _logger.LogInformation("User not found during experience fetch attempt.");
            return BadRequest(new MessageResponse("User not found"));
        }

        return Ok(new ExperienceResponse
        {
            LeveledUp = false,
            Level = userProfile.Level,
            Experience = userProfile.Experience
        });
    }
}
