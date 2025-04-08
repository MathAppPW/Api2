namespace MathAppApi.Features.UserProgress.Dtos;

public class ProgressDto
{
    public int Completed { get; set; } = 0;
    public int All { get; set; } = 0;
    public float ExercisesCompletedPercent {  get; set; } = 0;
}
