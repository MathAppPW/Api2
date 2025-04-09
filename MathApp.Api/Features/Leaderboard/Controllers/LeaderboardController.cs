using MathApp.Dal.Interfaces;
using MathAppApi.Features.Authentication.Dtos;
using MathAppApi.Features.Leaderboard.Dtos;
using MathAppApi.Features.Leaderboard.Services.Interfaces;
using MathAppApi.Features.UserProfile.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MathAppApi.Features.Leaderboard.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class LeaderboardController : ControllerBase
{
    private readonly ILeaderboardService _leaderboardService;
    private readonly ILeaderboardRepo _leaderboardRepo;
    private readonly IUserProfileRepo _userProfileRepo;

    private readonly ILogger<LeaderboardController> _logger;

    public LeaderboardController(ILeaderboardService leaderboardService, ILeaderboardRepo leaderboardRepo, IUserProfileRepo userProfileRepo, ILogger<LeaderboardController> logger)
    {
        _leaderboardService = leaderboardService;
        _leaderboardRepo = leaderboardRepo;
        _userProfileRepo = userProfileRepo;
        _logger = logger;
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] LeaderboardDto dto)
    {
        var userId = User.FindFirst("sub")?.Value;
        if (userId == null)
        {
            _logger.LogInformation("Leaderboard update attempt with no userId.");
            return Unauthorized();
        }

        await _leaderboardService.UpdateLeaderboard(dto);

        return Ok();
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<LeaderboardResponse>(StatusCodes.Status200OK)]
    [HttpGet("{name}/{count}/{onlyFriends}")]
    public async Task<IActionResult> Get(string name, bool onlyFriends, int count)
    {
        var userId = User.FindFirst("sub")?.Value;
        if (userId == null)
        {
            _logger.LogInformation("Ledaerboard fetch attempt with no userId.");
            return Unauthorized();
        }

        var userProfile = await _userProfileRepo.FindOneAsync(u => u.Id == userId);
        if (userProfile == null)
        {
            _logger.LogInformation("User not found during leaderboard fetch attempt.");
            return BadRequest(new MessageResponse("User not found"));
        }

        var leaderboard = await _leaderboardRepo.FindOneAsync(e => e.Id == name);
        if (leaderboard == null)
        {
            _logger.LogInformation("Leaderboards not found during leaderboard fetch attempt.");
            return NotFound();
        }

        if (onlyFriends)
        {
            leaderboard.Entries = await _leaderboardService.FilterByFriends(leaderboard.Entries, userId);
        }

        var myPosition = -1;
        for(int i = 0; i < leaderboard.Entries.Count; i++)
        {
            if (leaderboard.Entries[i].User == userProfile)
            {
                myPosition = i + 1;
                break;
            }
        }

        if(count < leaderboard.Entries.Count)
        {
            leaderboard.Entries = leaderboard.Entries.Slice(0, count);
        }

        return Ok(new LeaderboardResponse
        {
            Entries = leaderboard.Entries,
            MyPosition = myPosition
        });
    }
}
