using System.ComponentModel.DataAnnotations;

namespace MathAppApi.Features.UserProfile.Dtos;

public class SkinDto
{

    [Required]
    public int SkinId { get; set; } = -1;
}
