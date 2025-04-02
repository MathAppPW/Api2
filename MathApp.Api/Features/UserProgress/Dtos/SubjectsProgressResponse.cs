namespace MathAppApi.Features.UserProgress.Dtos;

public class SubjectsProgressResponse
{
    public Dictionary<Models.Subject, float> Progress { get; set; } = [];
}
