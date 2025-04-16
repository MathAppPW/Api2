namespace MathAppApi.Features.Exercise.Dtos;

public class SeriesResponse
{
    public List<SeriesResponseEntry> Series { get; set; } = [];
}

public class SeriesResponseEntry
{
    public int Id { get; set; }
    public int LessonId { get; set; }

    public List<SeriesResponseExercise> Exercises { get; set; } = [];
}

public class SeriesResponseExercise
{
    public int Id { get; set; }
    public string Contents { get; set; } = "";
}
