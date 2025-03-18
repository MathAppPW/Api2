using MathApp.Dal.Interfaces;
using MathAppApi.Features.Exercises.Dtos;
using MathAppApi.Features.Exercises.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MathAppApi.Features.Exercises.Controllers;

[Route("[controller]")]
[ApiController]
public class ExerciseController : ControllerBase
{
    private readonly IExerciseRepo _exerciseRepo;
    private readonly IExerciseSeriesRepo _exerciseSeriesRepo;

    public ExerciseController(IExerciseRepo exerciseRepo, IExerciseSeriesRepo exerciseSeriesRepo)
    {
        _exerciseRepo = exerciseRepo;
        _exerciseSeriesRepo = exerciseSeriesRepo;
    }

    [ProducesResponseType<ExerciseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get([FromRoute] int id)
    {
        var exercise = await _exerciseRepo.GetAsync(id);
        if (exercise == null)
            return NotFound();
        var dto = exercise.ToDto();
        return Ok(dto);
    }

    [ProducesResponseType<ExerciseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    [HttpGet("by_series")]
    public async Task<IActionResult> GetFromSeries([FromRoute] int seriesId, [FromRoute] int number)
    {
        var exerciseSeries =
            await _exerciseSeriesRepo.FindOneAsync(es => es.SeriesId == seriesId && es.SeriesOrder == number);
        if (exerciseSeries == null)
            return NotFound();
        await _exerciseSeriesRepo.LoadMemberAsync(exerciseSeries, es => es.Exercise);
        var dto = exerciseSeries.Exercise?.ToDto();
        if (dto == null)
            return NotFound();
        return Ok(dto);
    }
}