using System.ComponentModel.DataAnnotations;

namespace MathAppApi.Features.Authentication.Dtos;

public class ResetPasswordDto
{
    [Required] public string ResetId { get; set; } = "";
    [Required] public string Password { get; set; } = "";
}