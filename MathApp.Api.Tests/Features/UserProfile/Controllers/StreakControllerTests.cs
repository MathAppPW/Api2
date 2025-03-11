using MathApp.Dal.Interfaces;
using MathAppApi.Features.UserProfile.Controllers;
using MathAppApi.Features.UserProfile.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace MathApp.Api.Tests.Features.UserProfile.Controllers;

[TestFixture]
public class StreakControllerTests
{
    private Mock<IUserProfileRepo> _userRepoMock;
    private Mock<ILogger<StreakController>> _loggerMock;
    private StreakController _controller;

    [SetUp]
    public void SetUp()
    {
        _userRepoMock = new Mock<IUserProfileRepo>();
        _loggerMock = new Mock<ILogger<StreakController>>();
        _controller = new StreakController(_userRepoMock.Object, _loggerMock.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
                new Claim(ClaimTypes.NameIdentifier, "123")
            }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Test]
    public async Task Increase_ShouldReturnOk_WhenUserExists()
    {
        var userProfile = new Models.UserProfile { Id = "123", Streak = 3 };
        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Models.UserProfile, bool>>>())).ReturnsAsync(userProfile);
        _userRepoMock.Setup(repo => repo.UpdateAsync(It.IsAny<Models.UserProfile>())).Returns(Task.CompletedTask);

        var result = await _controller.Increase();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            var response = okResult.Value as StreakResponse;
            Assert.That(response, Is.Not.Null);
            if(response != null)
            {
                Assert.That(response.Streak, Is.EqualTo(4));
            }
        });
    }

    [Test]
    public async Task Increase_ShouldReturnBadRequest_WhenUserNotFound()
    {
        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Models.UserProfile, bool>>>()))
             .ReturnsAsync(default(Models.UserProfile));

        var result = await _controller.Increase();

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task Increase_ShouldReturnUnauthorized_WhenUserIdIsMissing()
    {
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();

        var result = await _controller.Increase();

        Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
    }

    [Test]
    public async Task Reset_ShouldReturnOk_WhenUserExists()
    {
        var userProfile = new Models.UserProfile { Id = "123", Streak = 5 };
        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Models.UserProfile, bool>>>())).ReturnsAsync(userProfile);
        _userRepoMock.Setup(repo => repo.UpdateAsync(It.IsAny<Models.UserProfile>())).Returns(Task.CompletedTask);

        var result = await _controller.Reset();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<OkResult>());
            Assert.That(userProfile.Streak, Is.EqualTo(0));
        });
    }

    [Test]
    public async Task Reset_ShouldReturnBadRequest_WhenUserNotFound()
    {
        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Models.UserProfile, bool>>>())).ReturnsAsync(default(Models.UserProfile));

        var result = await _controller.Reset();

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task Reset_ShouldReturnUnauthorized_WhenUserIdIsMissing()
    {
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();

        var result = await _controller.Reset();

        Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
    }

    [Test]
    public async Task Get_ShouldReturnStreak_WhenUserExists()
    {
        var userProfile = new Models.UserProfile { Id = "123", Streak = 10 };
        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Models.UserProfile, bool>>>())).ReturnsAsync(userProfile);

        var result = await _controller.Get();

        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            var response = okResult.Value as StreakResponse;
            Assert.That(response, Is.Not.Null);
            if (response != null)
            {
                Assert.That(response.Streak, Is.EqualTo(10));
            }
        });
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
