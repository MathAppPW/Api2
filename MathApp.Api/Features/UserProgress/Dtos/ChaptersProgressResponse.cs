namespace MathAppApi.Features.UserProgress.Dtos;

public class ChaptersProgressResponse
{
    public Dictionary<string, ProgressDto> Progress { get; set; } = [];
}
