using MathAppApi.Features.UserExerciseHistory.Dtos;
using Models;

namespace MathAppApi.Features.Progress.Services.Interfaces;

public interface IProgressService
{
    Task<List<ChapterProgressDto>> GetChapterProgressAsync(string userId);
    Task<List<LessonProgressDto>> GetLessonProgressAsync(string userId, string chapterName);
    Task<List<AllProgressDto>> GetSeriesProgressAsync(string userId, string chapterName, int lessonId);
}
