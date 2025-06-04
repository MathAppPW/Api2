namespace MathAppApi.Features.Rankings.Dtos;

public class Ranking
{
    public List<RankingEntry> RankingEntries { get; set; } = new();
    public DateTime FinishDate { get; set; }
    public int YourPosition { get; set; }
    public int YourScore { get; set; }
}

public record RankingEntry(string Username, int ProfileSkin, int Level, int Score);