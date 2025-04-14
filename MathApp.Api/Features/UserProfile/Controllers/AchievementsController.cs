using MathApp.Dal.Interfaces;
using MathAppApi.Features.Authentication.Dtos;
using MathAppApi.Features.UserProfile.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MathAppApi.Features.UserProfile.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class AchievementsController : ControllerBase
{
    private readonly IUserProfileRepo _userProfileRepo;

    private readonly ILogger<AchievementsController> _logger;

    public AchievementsController(IUserProfileRepo userProfileRepo, ILogger<AchievementsController> logger)
    {
        _userProfileRepo = userProfileRepo;
        _logger = logger;
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<AchievementsResponse>(StatusCodes.Status200OK)]
    [HttpGet("rocket")]
    public async Task<IActionResult> Rocket()
    {
        var userId = User.FindFirst("sub")?.Value;
        if (userId == null)
        {
            _logger.LogInformation("Rocket skin achievements fetch attempt with no userId.");
            return Unauthorized();
        }

        var userProfile = await _userProfileRepo.FindOneAsync(u => u.Id == userId);
        if (userProfile == null)
        {
            _logger.LogInformation("User not found during rocket skin achievements fetch attempt.");
            return BadRequest(new MessageResponse("User not found"));
        }

        return Ok(new AchievementsResponse
        {
            IsUnlocked = userProfile.AchievementsRocket
        });
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<AchievementsResponse>(StatusCodes.Status200OK)]
    [HttpGet("avatar")]
    public async Task<IActionResult> Avatar()
    {
        var userId = User.FindFirst("sub")?.Value;
        if (userId == null)
        {
            _logger.LogInformation("Avatar achievements fetch attempt with no userId.");
            return Unauthorized();
        }

        var userProfile = await _userProfileRepo.FindOneAsync(u => u.Id == userId);
        if (userProfile == null)
        {
            _logger.LogInformation("User not found during avatar achievements fetch attempt.");
            return BadRequest(new MessageResponse("User not found"));
        }

        return Ok(new AchievementsResponse
        {
            IsUnlocked = userProfile.AchievementsAvatar
        });
    }
}
