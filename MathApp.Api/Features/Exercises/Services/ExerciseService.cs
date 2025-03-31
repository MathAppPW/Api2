using MathApp.Dal.Interfaces;
using MathAppApi.Features.Exercise.Dtos;
using MathAppApi.Features.Exercise.Services.Interfaces;

namespace MathAppApi.Features.Exercise.Services;

public class ExerciseService : IExerciseService
{
    private readonly IChapterRepo _chapterRepo;
    private readonly ISeriesRepo _seriesRepo;

    private readonly ILogger<ExerciseService> _logger;

    public ExerciseService(ILogger<ExerciseService> logger, IChapterRepo chapterRepo, ISeriesRepo seriesRepo)
    {
        _logger = logger;
        _chapterRepo = chapterRepo;
        _seriesRepo = seriesRepo;
    }

    public async Task<ExerciseResponse> GetExercises(int seriesId)
    {
        var series = await _seriesRepo.FindOneAsync(e => e.Id == seriesId);
        if (series == null)
        {
            _logger.LogWarning("Series not found during exercises fetch attempt.");
            return new ExerciseResponse();
        }

        return new ExerciseResponse
        {
            Exercises = series.Exercises.ToList<Models.Exercise>()
        };
    }

    public async Task<SeriesResponse> GetSeries(SeriesDto dto)
    {
        var chapter = await _chapterRepo.FindOneAsync(e => e.Name == dto.ChapterName);
        if (chapter == null)
        {
            _logger.LogWarning("Series not found during exercises fetch attempt.");
            return new SeriesResponse();
        }

        var subject = chapter.Subjects.First(e => e.Name == dto.SubjectName);
        if (subject == null)
        {
            _logger.LogWarning("Subject not found during exercises fetch attempt.");
            return new SeriesResponse();
        }

        var lesson = subject.Lessons.First(e => e.Id == dto.LessonId);
        if (lesson == null)
        {
            _logger.LogWarning("Lesson not found during exercises fetch attempt.");
            return new SeriesResponse();
        }

        return new SeriesResponse
        {
            Series = lesson.Series.ToList<Models.Series>()
        };
    }
}
