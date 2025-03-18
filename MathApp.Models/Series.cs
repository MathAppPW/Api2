using Microsoft.EntityFrameworkCore;

namespace Models;

[Index(nameof(LessonId))]
public class Series
{
    public int Id { get; set; }
    public int LessonId{ get; set; }

    public ICollection<Exercise> Exercises { get; set; } = [];
    public Lesson? Lesson { get; set; }
}