using System.ComponentModel.DataAnnotations;

namespace MathAppApi.Features.UserProfile.Dtos;

public class BasicDto
{
    [Required]
    [StringLength(128)]
    public string UserId { get; set; } = "";
}
