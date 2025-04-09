using Dal;
using MathApp.Dal.Interfaces;
using MathAppApi.Features.Authentication.Dtos;
using MathAppApi.Features.Authentication.Services.Interfaces;
using MathAppApi.Features.UserProfile.Dtos;
using MathAppApi.Features.UserProfile.Extensions;
using MathAppApi.Features.UserProfile.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Security.Claims;

namespace MathAppApi.Features.UserProfile.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UserProfileController : ControllerBase
{
    private readonly IUserProfileRepo _userProfileRepo;

    private readonly ILivesService _livesService;

    private readonly ILogger<UserProfileController> _logger;

    public UserProfileController(IUserProfileRepo userProfileRepo, ILivesService livesService, ILogger<UserProfileController> logger)
    {
        _userProfileRepo = userProfileRepo;
        _livesService = livesService;
        _logger = logger;
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<UserProfileResponse>(StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userId = User.FindFirst("sub")?.Value;
        if (userId == null)
        {
            _logger.LogInformation("User profile fetch attempt with no userId.");
            return Unauthorized();
        }

        var userProfile = await _userProfileRepo.FindOneAsync(u => u.Id == userId);
        if (userProfile == null)
        {
            _logger.LogInformation("User not found during user profile fetch attempt.");
            return BadRequest(new MessageResponse("User not found"));
        }

        await _userProfileRepo.LoadMemberAsync(userProfile, u => u.User);

        var response = await userProfile.ToDto(_livesService);

        return Ok(response);
    }
}
