namespace Models;

public class ExerciseSeries
{
    public int Id { get; set; }
    public int SeriesId { get; set; }
    public int ExerciseId { get; set; }
    public int SeriesOrder { get; set; } //exercise index in the series
    
    public Series? Series { get; set; }
    public Exercise? Exercise { get; set; }
}