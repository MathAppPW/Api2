namespace MathAppApi.Features.UserProgress.Dtos;

public class SubjectsProgressResponse
{
    public Dictionary<string, ProgressDto> Progress { get; set; } = [];
}
