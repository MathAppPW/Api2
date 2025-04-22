namespace MathAppApi.Features.UserProgress.Dtos;

public class SubjectsProgressResponse
{
    public Dictionary<string, SubjectsProgressResponseEntry> Progress { get; set; } = [];
}

public class SubjectsProgressResponseEntry
{
    public int CurrentLesson { get; set; } = 1;
    public int SeriesCompleted { get; set; } = 0;
    public int SeriesAll { get; set; } = 0;
}
