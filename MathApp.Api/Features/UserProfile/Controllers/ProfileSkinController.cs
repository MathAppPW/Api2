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
public class ProfileSkinController : ControllerBase
{
    private readonly int skinCount = 10;
    private readonly IUserProfileRepo _userProfileRepo;

    private readonly ILogger<ProfileSkinController> _logger;

    public ProfileSkinController(IUserProfileRepo userProfileRepo, ILogger<ProfileSkinController> logger)
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
            _logger.LogInformation("Profile skin change attempt with invalid skin id.");
            return BadRequest(new MessageResponse("Invalid skin id"));
        }

        var userId = User.FindFirst("sub")?.Value;
        if (userId == null)
        {
            _logger.LogInformation("Profile skin change attempt with no userId.");
            return Unauthorized();
        }

        var userProfile = await _userProfileRepo.FindOneAsync(u => u.Id == userId);
        if (userProfile == null)
        {
            _logger.LogInformation("No user found during profile skin change attempt.");
            return BadRequest(new MessageResponse("User not found"));
        }

        userProfile.ProfileSkin = dto.SkinId;

        await _userProfileRepo.UpdateAsync(userProfile);

        return Ok();
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<SkinResponse>(StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userId = User.FindFirst("sub")?.Value;
        if (userId == null)
        {
            _logger.LogInformation("Profile skin fetch attempt with no userId.");
            return Unauthorized();
        }

        var userProfile = await _userProfileRepo.FindOneAsync(u => u.Id == userId);
        if (userProfile == null)
        {
            _logger.LogInformation("No user found during profile skin fetch attempt.");
            return BadRequest(new MessageResponse("User not found"));
        }

        return Ok(new SkinResponse{ SkinId = userProfile.ProfileSkin });
    }
}
