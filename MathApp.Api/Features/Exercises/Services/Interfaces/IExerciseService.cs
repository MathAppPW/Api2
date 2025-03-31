using MathAppApi.Features.Exercise.Dtos;

namespace MathAppApi.Features.Exercise.Services.Interfaces;

public interface IExerciseService
{
    Task<ExerciseResponse> GetExercises(int seriesId);
    Task<SeriesResponse> GetSeries(SeriesDto dto);
}
