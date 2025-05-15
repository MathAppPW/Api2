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

    [HttpGet("getGlobal/{count}")]
    public async Task<IActionResult> GetGlobal([FromRoute] int count)
    {
        if (count < 1)
            return BadRequest();
        
        var userId = User.FindFirst("sub")?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }
        
        var globalRanking = await _rankingService.GetGlobalRankingAsync(count, userId);
        return Ok(globalRanking);
    }

    [Authorize]
    [HttpGet("getFriend/{count}")]
    public async Task<IActionResult> GetFriend([FromRoute] int count)
    {
        var userId = User.FindFirst("sub")?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        var friendRanking = await _rankingService.GetFriendsRankingAsync(userId, count);
        return Ok(friendRanking);
    }
}