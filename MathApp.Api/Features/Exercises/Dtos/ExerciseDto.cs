namespace MathAppApi.Features.Exercise.Dtos;

public class ExerciseDto
{
    public string ChapterName { get; set; } = "";
    public string SubjectName { get; set; } = "";
    public int LessonId { get; set; }
    public int SeriesId { get; set; }
}
