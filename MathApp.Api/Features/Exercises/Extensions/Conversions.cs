using MathApp.Dal.Interfaces;
using MathAppApi.Features.Exercise.Dtos;

namespace MathAppApi.Features.Exercises.Extensions;

public static class Conversions
{
    public async static Task<SeriesResponse> ToDto(this ICollection<Models.Series> seriesCollection, ISeriesRepo seriesRepo)
    {
        var response = new SeriesResponse
        {
            Series = []
        };

        foreach(var series in seriesCollection)
        {
            var entry = new SeriesResponseEntry
            {
                Id = series.Id,
                LessonId = series.LessonId,
                Exercises = []
            };

            await seriesRepo.LoadCollectionAsync(series, e => e.Exercises);
            foreach (var exercise in series.Exercises)
            {
                var exerciseResponse = new SeriesResponseExercise
                {
                    Id = exercise.Id,
                    Contents = exercise.Contents
                };
                entry.Exercises.Add(exerciseResponse);
            }
            response.Series.Add(entry);
        }

        return response;
    }

    public static ExerciseResponse ToDto(this ICollection<Models.Exercise> exerciseCollection)
    {
        var response = new ExerciseResponse
        {
            Exercises = []
        };

        foreach (var exercise in exerciseCollection)
        {
            var exerciseResponse = new SeriesResponseExercise
            {
                Id = exercise.Id,
                Contents = exercise.Contents
            };
            response.Exercises.Add(exerciseResponse);
        }

        return response;
    }
}
