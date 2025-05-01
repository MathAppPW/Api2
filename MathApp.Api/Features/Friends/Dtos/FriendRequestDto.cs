namespace MathAppApi.Features.Friends.Dto;

public class FriendRequestDto
{
    public int Id { get; set; }
    public string SenderName { get; set; } = "";
    public string ReceiverName { get; set; } = "";
    public DateTime TimeStamp { get; set; }
    public int AvatarSkinId { get; set; }
}