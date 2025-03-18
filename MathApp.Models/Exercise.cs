using Microsoft.EntityFrameworkCore;

namespace Models;

public class Exercise
{
    public int Id { get; set; }
    public string Contents { get; set; } = "";

    public ICollection<ExerciseSeries> ExerciseSeries { get; set; } = [];
}