namespace MathAppApi.Features.UserProgress.Dtos;

public class LessonsProgressResponse
{
    public Dictionary<int, LessonProgressResponseEntry> Progress { get; set; } = [];
}

public class LessonProgressResponseEntry
{
    public int ExercisesCompleted { get; set; } = 0;
    public int ExercisesAll { get; set; } = 0;
}
