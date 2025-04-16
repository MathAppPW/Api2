namespace MathAppApi.Features.UserProgress.Dtos;

public class LessonsProgressResponse
{
    public Dictionary<int, ProgressDto> Progress { get; set; } = [];
}
