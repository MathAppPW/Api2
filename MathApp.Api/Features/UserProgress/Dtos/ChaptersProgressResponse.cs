namespace MathAppApi.Features.UserProgress.Dtos;

public class ChaptersProgressResponse
{
    public Dictionary<Models.Chapter, ProgressDto> Progress { get; set; } = [];
}
