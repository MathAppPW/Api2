namespace MathAppApi.Features.Friends.Dto;

public class FriendRequestDto
{
    public string SenderName { get; set; } = "";
    public string ReceiverName { get; set; } = "";
    public DateTime TimeStamp { get; set; }
}