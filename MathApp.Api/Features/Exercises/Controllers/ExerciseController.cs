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
    private readonly ILogger<HistoryController> _logger;

    private readonly IExerciseService _exerciseService;


    public ExerciseController(ILogger<HistoryController> logger, IExerciseService progressService)
    {
        _logger = logger;
        _exerciseService = progressService;
    }

    [ProducesResponseType<ExerciseResponse>(StatusCodes.Status200OK)]
    [HttpGet("{seriesId}")]
    public async Task<IActionResult> GetExercises(int seriesId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            _logger.LogInformation("Exercises fetch attempt with no userId.");
            return Unauthorized();
        }

        var response = await _exerciseService.GetExercises(seriesId);

        return Ok(response);
    }

    [ProducesResponseType<SeriesResponse>(StatusCodes.Status200OK)]
    [HttpGet("series/{chapterName}/{subjectName}/{lessonId}")]
    public async Task<IActionResult> GetSeries(string chapterName, string subjectName, int lessonId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            _logger.LogInformation("Series fetch attempt with no userId.");
            return Unauthorized();
        }

        var dto = new SeriesDto
        {
            ChapterName = chapterName,
            SubjectName = subjectName,
            LessonId = lessonId,
        };
        var response = await _exerciseService.GetSeries(dto);

        return Ok(response);
    }
}
