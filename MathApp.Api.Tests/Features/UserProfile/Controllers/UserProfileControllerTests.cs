using MathAppApi.Features.UserProfile.Controllers;
using MathAppApi.Features.UserProfile.Dtos;
using MathApp.Dal.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Linq.Expressions;
using Models;
using MathAppApi.Features.UserProfile.Services.Interfaces;

namespace MathApp.Api.Tests.Features.UserProfile.Controllers;

[TestFixture]
public class UserProfileControllerTests
{
    private Mock<IUserProfileRepo> _userRepoMock;
    private Mock<ILogger<UserProfileController>> _loggerMock;
    private Mock<ILivesService> _livesServiceMock;
    private UserProfileController _controller;

    [SetUp]
    public void SetUp()
    {
        _userRepoMock = new Mock<IUserProfileRepo>();
        _loggerMock = new Mock<ILogger<UserProfileController>>();
        _livesServiceMock = new Mock<ILivesService>();
        _controller = new UserProfileController(_userRepoMock.Object, _livesServiceMock.Object, _loggerMock.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim("sub", "123")
        ], "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Test]
    public async Task Get_ShouldReturnUserProfile_WhenUserExists()
    {
        var userProfile = new Models.UserProfile
        {
            Id = "123",
            User = new User()
            {
                Id = "123",
                Email = "test@mail.com",
                PasswordHash = "passwordHash",
                Username = "username"
            },
            Level = 5,
            Experience = 2500,
            Streak = 10,
            Lives = 3,
            LastLivesUpdate = DateTime.UtcNow,
            RocketSkin = 2,
            ProfileSkin = 1
        };
        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<Expression<Func<Models.UserProfile, bool>>>()))
            .ReturnsAsync(userProfile);

        var result = await _controller.Get();

        Assert.IsInstanceOf<OkObjectResult>(result);
        var okResult = (OkObjectResult)result;
        var response = okResult.Value as UserProfileResponse;
        Assert.That(response, Is.Not.Null);
        if (response != null)
        {
            Assert.That(response.Username, Is.EqualTo("username"));
            Assert.That(response.Level, Is.EqualTo(5));
            Assert.That(response.Experience, Is.EqualTo(2500));
            Assert.That(response.Streak, Is.EqualTo(10));
            Assert.That(response.Lives, Is.EqualTo(3));
            Assert.That(response.RocketSkin, Is.EqualTo(2));
            Assert.That(response.ProfileSkin, Is.EqualTo(1));
        }
    }

    [Test]
    public async Task Get_ShouldReturnBadRequest_WhenUserNotFound()
    {
        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Models.UserProfile, bool>>>())).ReturnsAsync(default(Models.UserProfile));

        var result = await _controller.Get();

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task Get_ShouldReturnUnauthorized_WhenUserIdIsMissing()
    {
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();

        var result = await _controller.Get();

        Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
    }
}
