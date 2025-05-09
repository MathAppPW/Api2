using MathApp.Dal.Interfaces;
using MathAppApi.Features.Exercise.Dtos;
using MathAppApi.Features.Exercise.Services.Interfaces;
using MathAppApi.Features.Exercises.Extensions;

namespace MathAppApi.Features.Exercise.Services;

public class ExerciseService : IExerciseService
{
    private readonly IChapterRepo _chapterRepo;
    private readonly ISubjectRepo _subjectRepo;
    private readonly ILessonRepo _lessonRepo;
    private readonly ISeriesRepo _seriesRepo;

    private readonly ILogger<ExerciseService> _logger;

    public ExerciseService(ILogger<ExerciseService> logger, IChapterRepo chapterRepo, ISubjectRepo subjectRepo, ILessonRepo lessonRepo, ISeriesRepo seriesRepo)
    {
        _logger = logger;
        _chapterRepo = chapterRepo;
        _subjectRepo = subjectRepo;
        _lessonRepo = lessonRepo;
        _seriesRepo = seriesRepo;
    }

    public async Task<ExerciseResponse> GetExercises(int seriesId)
    {
        var series = await _seriesRepo.FindOneAsync(e => e.Id == seriesId);
        if (series == null)
        {
            _logger.LogWarning("Series not found during exercises fetch attempt in GetExercises(int)");
            return new ExerciseResponse();
        }

        await _seriesRepo.LoadCollectionAsync(series, e => e.Exercises);

        return series.Exercises.ToDto();
    }

    public async Task<SeriesResponse> GetSeries(SeriesDto dto)
    {
        var chapter = await _chapterRepo.FindOneAsync(e => e.Name == dto.ChapterName);
        if (chapter == null)
        {
            _logger.LogWarning("Chapter not found during exercises fetch attempt.");
            return new SeriesResponse();
        }

        await _chapterRepo.LoadCollectionAsync(chapter, e => e.Subjects);
        var subject = chapter.Subjects.FirstOrDefault(e => e.Name == dto.SubjectName);
        if (subject == null)
        {
            _logger.LogWarning("Subject not found during exercises fetch attempt.");
            return new SeriesResponse();
        }

        await _subjectRepo.LoadCollectionAsync(subject, e => e.Lessons);
        var lesson = subject.Lessons.FirstOrDefault(e => e.Id == dto.LessonId);
        if (lesson == null)
        {
            _logger.LogWarning("Lesson not found during exercises fetch attempt.");
            return new SeriesResponse();
        }

        await _lessonRepo.LoadCollectionAsync(lesson, e => e.Series);

        var response = await lesson.Series.ToDto(_seriesRepo);
        return response;
    }

    public async Task<TheoryResponse> GetTheory(TheoryDto dto)
    {
        var chapter = await _chapterRepo.FindOneAsync(e => e.Name == dto.ChapterName);
        if (chapter == null)
        {
            _logger.LogWarning("Chapter not found during theory fetch attempt.");
            return new TheoryResponse();
        }

        await _chapterRepo.LoadCollectionAsync(chapter, e => e.Subjects);
        var subject = chapter.Subjects.FirstOrDefault(e => e.Name == dto.SubjectName);
        if (subject == null)
        {
            _logger.LogWarning("Subject not found during theory fetch attempt.");
            return new TheoryResponse();
        }

        await _subjectRepo.LoadMemberAsync(subject, e => e.Theory);
        if (subject.Theory == null)
        {
            _logger.LogWarning("Theory not found during theory fetch attempt.");
            return new TheoryResponse();
        }

        return new TheoryResponse()
        {
            Content = subject.Theory.Content
        };
    }
}
