namespace MathAppApi.Features.Authentication.DataStorages;

public interface IPasswordResetDataStorage
{
    //returns id which can be used to get user from the storage
    string RegisterPasswordReset(string userId);
    string? GetUserId(string resetId);
}