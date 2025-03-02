using System.ComponentModel.DataAnnotations;
using MathAppApi.Features.Authentication.Dtos;
using MathAppApi.Features.Authentication.Services.Interfaces;
using Models;

namespace MathAppApi.Features.Authentication.Services;

public class UserDataValidator : IUserDataValidator
{
    public (bool, string) IsUserDataValid(RegisterDto data)
    {
        var (isValid, message) = IsPasswordValid(data.Password);
        if (!isValid)
            return (false, message);
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(data);

        isValid = Validator.TryValidateObject(data, validationContext, validationResults, true);
        if (!isValid)
            return (false, validationResults[0].ErrorMessage ?? "Unspecified error in data");
        return (true, "");
    }

    private (bool, string) IsPasswordValid(string password)
    {
        var hasLowerLetter = password.Any(char.IsLower);
        var hasUpperLetter = password.Any(char.IsUpper);
        var hasDigit = password.Any(char.IsDigit);
        var hasSpecial = password.Any(c => !char.IsLetterOrDigit(c));

        if (!hasLowerLetter)
            return (false, "Password must contain a lower letter.");
        if (!hasUpperLetter)
            return (false, "Password must contain an upper letter.");
        if (!hasDigit)
            return (false, "Password must contain a digit.");
        if (!hasSpecial)
            return (false, "Password must contain a special character.");
        return (true, "");
    }
}