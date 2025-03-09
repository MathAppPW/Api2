using MathAppApi.Features.Authentication.Dtos;

namespace MathAppApi.Features.Authentication.Services.Interfaces;

public interface IUserDataValidator
{
    //returns empty string when data is valid
    (bool, string) IsUserDataValid(RegisterDto data);
    (bool, string) IsPasswordValid(string password);
}