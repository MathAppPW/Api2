using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models;

public class FriendRequest
{
    [Required] public int Id { get; set; }
    [Required] [ForeignKey(nameof(Sender))] public string SenderUserId { get; set; } = "";
    [Required] [ForeignKey(nameof(Receiver))] public string ReceiverUserId { get; set; } = "";
    [Required] public DateTime TimeStamp { get; set; }
    
    public User? Sender { get; set; }
    public User? Receiver { get; set; }
}