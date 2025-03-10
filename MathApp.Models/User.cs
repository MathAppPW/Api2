using System.ComponentModel.DataAnnotations;

namespace Models;

public class User
{
    [Required] public string Id { get; set; } = "";
    [Required] public string Email { get; set; } = "";
    [Required] public string Username { get; set; } = "";
    [Required] public string PasswordHash { get; set; } = "";
    [Required] public int Level { get; set; } = 1;
    [Required] public int Experience { get; set; } = 0;
    [Required] public int Streak { get; set; } = 0;
    [Required] public int Lives { get; set; } = 10;
    [Required] public DateTime LastLivesUpdate { get; set; } = DateTime.UtcNow;
    [Required] public int RocketSkin { get; set; } = 0;
    [Required] public int ProfileSkin { get; set; } = 0;
    public string? TokenVersion { get; set; } = "";
}