using System.ComponentModel.DataAnnotations;

namespace MathAppApi.Features.UserProfile.Dtos;

public class LivesResponse
{
    [Required]
    public int Lives { get; set; } = 0;
    [Required]
    public int SecondsToHeal { get; set; } = 0;
}
