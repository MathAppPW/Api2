namespace MathAppApi.Features.Exercises.Dtos;

public class ChapterDto
{
    public string Name { get; set; } = "";

    public List<SubjectHeaderDto> Subjects { get; set; } = [];
}