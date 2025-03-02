using System.ComponentModel.DataAnnotations;

namespace MathAppApi.Features.Authentication.Dtos;

public class RegisterDto
{
    [Required]
    [StringLength(128, MinimumLength = 8, ErrorMessage = "Invalid username length")]
    public string Username { get; set; } = "";

    [Required]
    [StringLength(320, MinimumLength = 6, ErrorMessage = "Invalid email length")] //320 is maximum email length
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = "";
    
    [Required]
    [StringLength(128, MinimumLength = 6, ErrorMessage = "Invalid password length")]
    public string Password { get; set; } = "";
}