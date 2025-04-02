using System.ComponentModel.DataAnnotations;

namespace MathAppApi.Features.Authentication.Dtos;

public class RemovePasswordDto
{
    [Required] public string Password { get; set; } = "";
}