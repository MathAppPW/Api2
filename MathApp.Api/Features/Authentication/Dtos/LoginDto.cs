using System.ComponentModel.DataAnnotations;

namespace MathAppApi.Features.Authentication.Dtos;

public class LoginDto
{
    [Required]
    [StringLength(128, MinimumLength = 8)]
    public string Username { get; set; } = "";

    [Required]
    [StringLength(128, MinimumLength = 8)]
    public string Password { get; set; } = "";
}