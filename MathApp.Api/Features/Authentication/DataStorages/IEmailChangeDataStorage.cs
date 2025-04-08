namespace MathAppApi.Features.Authentication.DataStorages;

public interface IEmailChangeDataStorage
{
    string RegisterEmailUpdate(string userId, string newMail);
    (string userId, string newMail)? GetEmailUpdateRequest(string requestId);
}