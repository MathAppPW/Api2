namespace MathAppApi.Features.UserProgress.Dtos;

public class ChaptersProgressResponse
{
    public Dictionary<string, ChapterProgressResponseEntry> Progress { get; set; } = [];
}

public class ChapterProgressResponseEntry
{
    public int SubjectsCompleted { get; set; } = 0;
    public int SubjectsAll { get; set; } = 0;
}
