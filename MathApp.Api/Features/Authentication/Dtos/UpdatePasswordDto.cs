using System.ComponentModel.DataAnnotations;

namespace MathAppApi.Features.Authentication.Dtos;

public class UpdatePasswordDto
{
    [Required] public string OldPassword { get; set; } = "";
    [Required] public string NewPassword { get; set; } = "";
}