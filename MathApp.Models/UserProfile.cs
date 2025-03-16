using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models;

public class UserProfile
{
    [Required]
    public string Id { get; set; } = "";
    [Required]
    public int Level { get; set; } = 1;
    [Required]
    public int Experience { get; set; } = 0;
    [Required]
    public int Streak { get; set; } = 0;
    [Required]
    public int Lives { get; set; } = 10;
    [Required]
    public DateTime LastLivesUpdate { get; set; } = DateTime.UtcNow;
    [Required]
    public int RocketSkin { get; set; } = 0;
    [Required]
    public int ProfileSkin { get; set; } = 0;
    [Required]
    public List<string> History { get; set; } = [];
}