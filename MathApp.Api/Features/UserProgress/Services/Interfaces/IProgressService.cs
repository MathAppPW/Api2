using MathAppApi.Features.UserProgress.Dtos;
using Models;

namespace MathAppApi.Features.UserProgress.Services.Interfaces;

public interface IProgressService
{
    Task<ChaptersProgressResponse> GetChaptersProgressAsync(Models.UserProfile userProfile);
    Task<SubjectsProgressResponse> GetSubjectsProgressAsync(Models.UserProfile userProfile, string chapterName);
    Task<LessonsProgressResponse> GetLessonsProgressAsync(Models.UserProfile userProfile, string chapterName, string subjectName);
}
