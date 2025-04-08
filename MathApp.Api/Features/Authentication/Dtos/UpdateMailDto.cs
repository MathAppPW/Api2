using System.ComponentModel.DataAnnotations;

namespace MathAppApi.Features.Authentication.Dtos;

public class UpdateMailDto
{
    [EmailAddress] [Required] public string NewMail { get; set; } = "";
}