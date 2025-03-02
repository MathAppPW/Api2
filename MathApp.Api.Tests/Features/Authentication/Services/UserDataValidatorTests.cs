using MathAppApi.Features.Authentication.Dtos;
using MathAppApi.Features.Authentication.Services;

namespace MathApp.Api.Tests.Features.Authentication.Services;

[TestFixture]
public class UserDataValidatorTests
{
    private UserDataValidator _userDataValidator;

    [SetUp]
    public void SetUp()
    {
        _userDataValidator = new UserDataValidator();
    }

    [Test]
    [TestCase("password123!")]
    [TestCase("PASSWORD123!")]
    [TestCase("PaSsWoRd1234")]
    [TestCase("12345678!!")]
    [TestCase("weakpassword")]
    [TestCase("PaSSworDd!?;")]
    [TestCase("")]
    [TestCase("short")]
    [TestCase("          ")]
    [TestCase("S!1m")]
    public void ShouldReturnFalse_WhenPasswordIsIncorrect(string password)
    {
        var (isValid, message) = ValidateForData("test@mail.com", "test_user1234", password);
        Assert.Multiple(() =>
        {
            Assert.That(isValid, Is.False);
            Assert.That(message, Is.Not.Empty.And.Not.Null);
        });
    }

    //very lax tests, only reliable method of email testing is sending them
    [Test]
    [TestCase("")]
    [TestCase("@mail.com")]
    [TestCase("mail")]
    public void ShouldReturnFalse_WhenEmailIsInvalid(string email)
    {
        var (isValid, message) = ValidateForData(email, "test_user1234", "ValidPassword1234!");
        Assert.Multiple(() =>
        {
            Assert.That(isValid, Is.False);
            Assert.That(message, Is.Not.Empty);
        });
    }

    [Test]
    [TestCase("")]
    [TestCase("shrt")]
    public void ShouldReturnFalse_WhenUsernameIsInvalid(string username)
    {
        var (isValid, message) = ValidateForData("valid@mail.com", username, "ValidPassword1234!");
        Assert.Multiple(() =>
        {
            Assert.That(isValid, Is.False);
            Assert.That(message, Is.Not.Empty);
        });
    }
    
    [Test]
    public void ShouldReturnTrue_WhenAllDataIsValid()
    {
        var (isValid, message) = ValidateForData("test@mail.com", "test_user1234", "ValidPassword1234!");
        Assert.Multiple(() =>
        {
            Assert.That(isValid, Is.True);
            Assert.That(message, Is.Empty);
        });
    }

    private (bool, string) ValidateForData(string email, string username, string password)
    {
        var dto = new RegisterDto()
        {
            Email = email,
            Username = username,
            Password = password
        };
        return _userDataValidator.IsUserDataValid(dto);
    }
}