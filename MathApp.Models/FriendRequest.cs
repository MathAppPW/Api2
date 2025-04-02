using System.ComponentModel.DataAnnotations;

namespace Models;

public class FriendRequest
{
    [Required] public int Id { get; set; }
    [Required] public string SenderUserId { get; set; } = "";
    [Required] public string ReceiverUserId { get; set; } = "";
    [Required] public DateTime TimeStamp { get; set; }
    
    public User? Sender { get; set; }
    public User? Receiver { get; set; }
}