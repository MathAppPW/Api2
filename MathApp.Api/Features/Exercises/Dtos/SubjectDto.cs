namespace MathAppApi.Features.Exercises.Dtos;

public class SubjectDto
{
    public string Name { get; set; } = "";

    public List<LessonHeaderDto> Lessons { get; set; } = [];
}