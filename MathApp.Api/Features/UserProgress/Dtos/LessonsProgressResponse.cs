namespace MathAppApi.Features.UserProgress.Dtos;

public class LessonsProgressResponse
{
    public Dictionary<Models.Lesson, ProgressDto> Progress { get; set; } = [];
}
