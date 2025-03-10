using System.ComponentModel.DataAnnotations;

namespace MathAppApi.Features.UserProfile.Dtos;

public class SkinDto
{
    [Required]
    [StringLength(128)]
    public string UserId { get; set; } = "";

    [Required]
    public int SkinId { get; set; } = -1;
}
