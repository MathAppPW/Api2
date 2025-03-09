using MathAppApi.Shared.Emails.Interfaces;

namespace MathAppApi.Shared.Emails;

public class FakeEmailService : IEmailService
{
    public async Task SendEmail(string address, string content)
    {
        var text = $"""
                    -------------------------
                    EMAIL SENT TO {address}
                    -------------------------
                    {content}
                    -------------------------
                    """;
        Console.WriteLine(text);
    }
}