using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models;

public class Friendship
{
    [Required]
    public string Id { get; set; } = "";
    [ForeignKey(nameof(User1))] public string UserId1 { get; set; } = "";
    [ForeignKey(nameof(User2))] public string UserId2 { get; set; } = "";
    
    public User? User1 { get; set; }
    public User? User2 { get; set; }
}