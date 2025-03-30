using Dal;
using MathApp.Dal.Interfaces;
using MathAppApi.Features.Authentication.Dtos;
using MathAppApi.Features.UserExerciseHistory.Controllers;
using MathAppApi.Features.Exercise.Dtos;
using MathAppApi.Features.Exercise.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MathAppApi.Features.UserProgress.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ExerciseController : ControllerBase
{
    private readonly IUserProfileRepo _userProfileRepo;

    private readonly ILogger<HistoryController> _logger;

    private readonly IExerciseService _exerciseService;


    public ExerciseController(IUserProfileRepo userProfileRepo, ILogger<HistoryController> logger, IExerciseService progressService)
    {
        _userProfileRepo = userProfileRepo;
        _logger = logger;
        _exerciseService = progressService;
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ExerciseResponse>(StatusCodes.Status200OK)]
    [HttpGet("{chapterName}/{subjectName}/{lessonId}/{seriesId}")]
    public async Task<IActionResult> GetExercises(string chapterName, string subjectName, int lessonId, int seriesId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            _logger.LogInformation("Exercises fetch attempt with no userId.");
            return Unauthorized();
        }

        var userProfile = await _userProfileRepo.FindOneAsync(u => u.Id == userId);
        if (userProfile == null)
        {
            _logger.LogWarning("User not found during exercises fetch attempt.");
            return BadRequest(new MessageResponse("User not found"));
        }

        var dto = new ExerciseDto
        {
            ChapterName = chapterName,
            SubjectName = subjectName,
            LessonId = lessonId,
            SeriesId = seriesId
        };
        var response = await _exerciseService.GetExercises(userProfile, dto);

        return Ok(response);
    }
}
