using MathAppApi.Features.Rankings.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MathAppApi.Features.Rankings.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class RankingController : ControllerBase
{
    private readonly IRankingService _rankingService;
    
    public RankingController(IRankingService rankingService)
    {
        _rankingService = rankingService;
    }

    [HttpGet("getGlobal")]
    public async Task<IActionResult> GetGlobal()
    {
        var globalRanking = await _rankingService.GetGlobalRankingAsync(10);
        return Ok(globalRanking);
    }

    [Authorize]
    [HttpGet("getFriend")]
    public async Task<IActionResult> GetFriend()
    {
        var userId = User.FindFirst("sub")?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        var friendRanking = await _rankingService.GetFriendsRankingAsync(userId, 10);
        return Ok(friendRanking);
    }
}