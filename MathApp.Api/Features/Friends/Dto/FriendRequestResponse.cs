using System.ComponentModel.DataAnnotations;

namespace MathAppApi.Features.Friends.Dto;

public class FriendRequestResponse
{
    [Required] public int RequestId { get; set; }
    [Required] public bool DidAccept { get; set; }
}