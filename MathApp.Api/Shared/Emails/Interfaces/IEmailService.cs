namespace MathAppApi.Shared.Emails.Interfaces;

public interface IEmailService
{
    Task SendEmail(string address, string content);
}