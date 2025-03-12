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
public class RocketSkinController : ControllerBase
{
    private readonly int skinCount = 5;
    private readonly IUserProfileRepo _userProfileRepo;

    private readonly ILogger<RocketSkinController> _logger;

    public RocketSkinController(IUserProfileRepo userProfileRepo, ILogger<RocketSkinController> logger)
    {
        _userProfileRepo = userProfileRepo;
        _logger = logger;
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] SkinDto dto)
    {
        if(dto.SkinId < 0 || dto.SkinId >= skinCount)
        {
            _logger.LogInformation("Rocket skin change attempt with invalid skin id.");
            return BadRequest(new MessageResponse("Invalid skin id"));
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            _logger.LogInformation("Rocket skin change attempt with no userId.");
            return Unauthorized();
        }

        var userProfile = await _userProfileRepo.FindOneAsync(u => u.Id == userId);
        if (userProfile == null)
        {
            _logger.LogInformation("No user found during rocket skin change attempt.");
            return BadRequest(new MessageResponse("User not found"));
        }

        userProfile.RocketSkin = dto.SkinId;

        await _userProfileRepo.UpdateAsync(userProfile);

        return Ok();
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<SkinResponse>(StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            _logger.LogInformation("Rocket skin fetch attempt with no userId.");
            return Unauthorized();
        }

        var userProfile = await _userProfileRepo.FindOneAsync(u => u.Id == userId);
        if (userProfile == null)
        {
            _logger.LogInformation("No user found during rocket skin fetch attempt.");
            return BadRequest(new MessageResponse("User not found"));
        }

        return Ok(new SkinResponse{ SkinId = userProfile.RocketSkin });
    }
}
