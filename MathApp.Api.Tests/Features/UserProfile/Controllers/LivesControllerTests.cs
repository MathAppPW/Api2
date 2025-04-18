using MathAppApi.Features.UserProfile.Controllers;
using MathAppApi.Features.UserProfile.Dtos;
using MathApp.Dal.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using MathAppApi.Features.UserProfile.Services.Interfaces;

namespace MathApp.Api.Tests.Features.UserProfile.Controllers;

[TestFixture]
public class LivesControllerTests
{
    private Mock<IUserProfileRepo> _userRepoMock;
    private Mock<ILogger<LivesController>> _loggerMock;
    private Mock<ILivesService> _livesServiceMock;
    private LivesController _controller;

    [SetUp]
    public void SetUp()
    {
        _userRepoMock = new Mock<IUserProfileRepo>();
        _loggerMock = new Mock<ILogger<LivesController>>();
        _livesServiceMock = new Mock<ILivesService>();
        _controller = new LivesController(_userRepoMock.Object, _livesServiceMock.Object, _loggerMock.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
                new Claim("sub", "123")
            }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Test]
    public async Task UpdateLives_ShouldReturnOk_WhenUserExists()
    {
        var userProfile = new Models.UserProfile { Id = "123", Lives = 5, LastLivesUpdate = DateTime.UtcNow.AddMinutes(-30) };
        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Models.UserProfile, bool>>>())).ReturnsAsync(userProfile);
        _userRepoMock.Setup(repo => repo.UpdateAsync(It.IsAny<Models.UserProfile>())).Returns(Task.CompletedTask);

        var result = await _controller.UpdateLives();

        Assert.IsInstanceOf<OkObjectResult>(result);
        var okResult = (OkObjectResult)result;
        var response = okResult.Value as LivesResponse;
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Lives, Is.GreaterThanOrEqualTo(5));
        Assert.That(response.SecondsToHeal, Is.GreaterThanOrEqualTo(0));
    }

    [Test]
    public async Task UpdateLives_ShouldReturnBadRequest_WhenUserNotFound()
    {
        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Models.UserProfile, bool>>>())).ReturnsAsync(default(Models.UserProfile));

        var result = await _controller.UpdateLives();

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task UpdateLives_ShouldReturnUnauthorized_WhenUserIdIsMissing()
    {
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();

        var result = await _controller.UpdateLives();

        Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
    }

    [Test]
    public async Task Damage_ShouldReturnOk_WhenUserExists()
    {
        var userProfile = new Models.UserProfile { Id = "123", Lives = 5 };
        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Models.UserProfile, bool>>>())).ReturnsAsync(userProfile);
        _userRepoMock.Setup(repo => repo.UpdateAsync(It.IsAny<Models.UserProfile>())).Returns(Task.CompletedTask);

        var result = await _controller.Damage();

        Assert.IsInstanceOf<OkObjectResult>(result);
        var okResult = (OkObjectResult)result;
        var response = okResult.Value as LivesResponse;
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Lives, Is.EqualTo(4));
    }

    [Test]
    public async Task Damage_ShouldReturnBadRequest_WhenUserNotFound()
    {
        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Models.UserProfile, bool>>>())).ReturnsAsync(default(Models.UserProfile));

        var result = await _controller.Damage();

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task Damage_ShouldReturnUnauthorized_WhenUserIdIsMissing()
    {
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();

        var result = await _controller.Damage();

        Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
    }

    [Test]
    public async Task Heal_ShouldReturnOk_WhenUserExists()
    {
        var userProfile = new Models.UserProfile { Id = "123", Lives = 4 };
        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Models.UserProfile, bool>>>())).ReturnsAsync(userProfile);
        _userRepoMock.Setup(repo => repo.UpdateAsync(It.IsAny<Models.UserProfile>())).Returns(Task.CompletedTask);

        var result = await _controller.Heal();

        Assert.IsInstanceOf<OkObjectResult>(result);
        var okResult = (OkObjectResult)result;
        var response = okResult.Value as LivesResponse;
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Lives, Is.EqualTo(5));
    }

    [Test]
    public async Task Heal_ShouldReturnBadRequest_WhenUserNotFound()
    {
        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Models.UserProfile, bool>>>())).ReturnsAsync((Models.UserProfile?)null);

        var result = await _controller.Heal();

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task Heal_ShouldReturnUnauthorized_WhenUserIdIsMissing()
    {
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();

        var result = await _controller.Heal();

        Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
    }
}
