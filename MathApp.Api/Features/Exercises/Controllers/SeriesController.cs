using MathApp.Dal.Interfaces;
using MathAppApi.Features.Exercises.Dtos;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace MathAppApi.Features.Exercises.Controllers;

[ApiController]
[Route("[controller]")]
public class SeriesController : ControllerBase
{
    private readonly ISeriesRepo _seriesRepo;
    private readonly IExerciseSeriesRepo _exerciseSeriesRepo;

    public SeriesController(ISeriesRepo seriesRepo, IExerciseSeriesRepo exerciseSeriesRepo)
    {
        _seriesRepo = seriesRepo;
        _exerciseSeriesRepo = exerciseSeriesRepo;
    }

    [ProducesResponseType<SeriesDto>(StatusCodes.Status200OK)]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get([FromRoute] int id)
    {
        var series = await _seriesRepo.GetAsync(id);
        if (series == null)
            return NotFound();
        await _seriesRepo.LoadCollectionAsync(series, s => s.ExerciseSeries);
        var dto = SeriesToExerciseDto(series);
        return Ok(dto);
    }

    private SeriesDto SeriesToExerciseDto(Series series)
    {
        var dto = new SeriesDto()
        {
            Id = series.Id,
            Exercises = series.ExerciseSeries
                .OrderBy(e => e.SeriesOrder)
                .Select(ExerciseSeriesToExerciseHeaderDto)
                .ToList(),
        };
        return dto;
    }
    
    private ExerciseHeaderDto ExerciseSeriesToExerciseHeaderDto(ExerciseSeries exerciseSeries)
    {
        return new ExerciseHeaderDto()
        {
            Id = exerciseSeries.ExerciseId,
        };
    }
}