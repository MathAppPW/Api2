using Dal;
using MathApp.Dal.Interfaces;
using MathAppApi.Features.Exercises.Dtos;
using MathAppApi.Features.Exercises.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MathAppApi.Features.Exercises.Controllers;

[ApiController]
[Route("[controller]")]
public class LessonController : ControllerBase
{
    private readonly ILessonRepo _lessonRepo;
    
    public LessonController(LessonRepo lessonRepo)
    {
        _lessonRepo = lessonRepo;
    }

    [ProducesResponseType<LessonDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var lesson = await _lessonRepo.GetAsync(id);
        if (lesson == null)
            return NotFound();
        await _lessonRepo.LoadCollectionAsync(lesson, l => l.Series);
        return Ok(lesson.ToLessonDto());
    }
}