namespace MathAppApi.Features.UserProgress.Dtos;

public class ChaptersProgressResponse
{
    public Dictionary<Models.Chapter, float> Progress { get; set; } = [];
}
