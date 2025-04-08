using System.ComponentModel.DataAnnotations;

namespace MathAppApi.Features.Authentication.Dtos;

public class AcceptMailUpdateDto
{
    [Required] public string Code { get; set; } = "";
}