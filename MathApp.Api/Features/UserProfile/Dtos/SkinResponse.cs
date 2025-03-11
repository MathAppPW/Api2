using System.ComponentModel.DataAnnotations;

namespace MathAppApi.Features.UserProfile.Dtos;

public class SkinResponse
{
    [Required]
    public int SkinId { get; set; } = 0;
}
