namespace MathAppApi.Features.Exercises.Dtos;

public class LessonDto
{
    public int Id { get; set; }
    public List<SeriesHeaderDto> Series { get; set; } = [];
}