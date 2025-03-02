namespace MathAppApi.Features.Authentication.Dtos;

public class MessageResponse
{
    public string Message { get; set; }

    public MessageResponse(string message)
    {
        Message = message;
    }
}