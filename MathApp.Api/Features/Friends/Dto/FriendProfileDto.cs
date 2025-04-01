using MathAppApi.Features.UserExerciseHistory.Dtos;

namespace MathAppApi.Features.Friends.Dto;

public class FriendProfileDto
{
    public string Username { get; set; } = "";
    public int AvatarId { get; set; }
    public int RocketShipId { get; set; }
    public int Level { get; set; }
    public StreakResponse? CurrentStreak { get; set; }
}