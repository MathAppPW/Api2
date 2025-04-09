using System.ComponentModel.DataAnnotations;

namespace MathAppApi.Features.Authentication.Dtos;

public class UpdateUsernameDto
{
    [Required] public string NewUsername { get; set; } = "";
}