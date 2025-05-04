namespace MathAppApi.Features.Rankings.Dtos;

public class Ranking
{
    public List<RankingEntry> RankingEntries { get; set; } = new();
}

public record RankingEntry(string Username, int AvatarId, int Score);