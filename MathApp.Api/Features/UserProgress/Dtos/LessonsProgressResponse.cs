namespace MathAppApi.Features.UserProgress.Dtos;

public class LessonsProgressResponse
{
    public Dictionary<Models.Lesson, float> Progress { get; set; } = [];
}
