using Microsoft.EntityFrameworkCore;

namespace Models;

[Index(nameof(LessonId))]
public class Series
{
    public int Id { get; set; }
    public int LessonId{ get; set; }

    public ICollection<ExerciseSeries> ExerciseSeries { get; set; } = [];
    public Lesson? Lesson { get; set; }
}