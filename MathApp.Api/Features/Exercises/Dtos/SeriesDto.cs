namespace MathAppApi.Features.Exercises.Dtos;

public class SeriesDto
{
    public int Id { get; set; }

    public List<ExerciseHeaderDto> Exercises { get; set; } = [];
}