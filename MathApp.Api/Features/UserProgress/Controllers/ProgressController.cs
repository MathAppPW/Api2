using Dal;
using MathApp.Dal.Interfaces;
using MathAppApi.Features.Authentication.Dtos;
using MathAppApi.Features.UserExerciseHistory.Controllers;
using MathAppApi.Features.UserProgress.Dtos;
using MathAppApi.Features.UserProgress.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MathAppApi.Features.UserProgress.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ProgressController : ControllerBase
{
    private readonly IUserProfileRepo _userProfileRepo;

    private readonly ILogger<HistoryController> _logger;

    private readonly IProgressService _progressService;


    public ProgressController(IUserProfileRepo userProfileRepo, ILogger<HistoryController> logger, IProgressService progressService)
    {
        _userProfileRepo = userProfileRepo;
        _logger = logger;
        _progressService = progressService;
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ChaptersProgressResponse>(StatusCodes.Status200OK)]
    [HttpGet("chapters")]
    public async Task<IActionResult> GetChapters()
    {
        var userId = User.FindFirst("sub")?.Value;
        if (userId == null)
        {
            _logger.LogInformation("Chapters progress fetch attempt with no userId.");
            return Unauthorized();
        }

        var userProfile = await _userProfileRepo.FindOneAsync(u => u.Id == userId);
        if (userProfile == null)
        {
            _logger.LogWarning("User not found during chapters progress fetch attempt.");
            return BadRequest(new MessageResponse("User not found"));
        }

        var response = await _progressService.GetChaptersProgressAsync(userProfile);

        return Ok(response);
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<SubjectsProgressResponse>(StatusCodes.Status200OK)]
    [HttpPost("subjects")]
    public async Task<IActionResult> GetSubjects([FromBody] SubjectsProgressDto dto)
    {
        var userId = User.FindFirst("sub")?.Value;
        if (userId == null)
        {
            _logger.LogInformation("Subjects progress fetch attempt with no userId.");
            return Unauthorized();
        }

        var userProfile = await _userProfileRepo.FindOneAsync(u => u.Id == userId);
        if (userProfile == null)
        {
            _logger.LogWarning("User not found during subjects progress fetch attempt.");
            return BadRequest(new MessageResponse("User not found"));
        }

        var response = await _progressService.GetSubjectsProgressAsync(userProfile, dto.ChapterName);

        return Ok(response);
    }
}
