using MathAppApi.Features.UserProfile.Controllers;
using MathAppApi.Features.UserProfile.Dtos;
using MathApp.Dal.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace MathApp.Api.Tests.Features.UserProfile.Controllers;

[TestFixture]
public class RocketSkinControllerTests
{
    private Mock<IUserProfileRepo> _userRepoMock;
    private Mock<ILogger<RocketSkinController>> _loggerMock;
    private RocketSkinController _controller;

    [SetUp]
    public void SetUp()
    {
        _userRepoMock = new Mock<IUserProfileRepo>();
        _loggerMock = new Mock<ILogger<RocketSkinController>>();
        _controller = new RocketSkinController(_userRepoMock.Object, _loggerMock.Object);

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
    public async Task Post_ShouldReturnOk_WhenValidSkinId()
    {
        var userProfile = new Models.UserProfile { Id = "123", RocketSkin = 1 };
        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Models.UserProfile, bool>>>())).ReturnsAsync(userProfile);
        _userRepoMock.Setup(repo => repo.UpdateAsync(It.IsAny<Models.UserProfile>())).Returns(Task.CompletedTask);

        var result = await _controller.Post(new SkinDto { SkinId = 2 });

        Assert.IsInstanceOf<OkResult>(result);
        Assert.That(userProfile.RocketSkin, Is.EqualTo(2));
    }

    [Test]
    public async Task Post_ShouldReturnBadRequest_WhenInvalidSkinId()
    {
        var result = await _controller.Post(new SkinDto { SkinId = 10 });

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task Post_ShouldReturnBadRequest_WhenUserNotFound()
    {
        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Models.UserProfile, bool>>>())).ReturnsAsync(default(Models.UserProfile));

        var result = await _controller.Post(new SkinDto { SkinId = 2 });

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task Post_ShouldReturnUnauthorized_WhenUserIdIsMissing()
    {
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();

        var result = await _controller.Post(new SkinDto { SkinId = 2 });

        Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
    }

    [Test]
    public async Task Get_ShouldReturnSkinId_WhenUserExists()
    {
        var userProfile = new Models.UserProfile { Id = "123", RocketSkin = 3 };
        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Models.UserProfile, bool>>>())).ReturnsAsync(userProfile);

        var result = await _controller.Get();

        Assert.IsInstanceOf<OkObjectResult>(result);
        var okResult = (OkObjectResult)result;
        var response = okResult.Value as SkinResponse;
        Assert.That(response, Is.Not.Null);
        if (response != null)
        {
            Assert.That(response.SkinId, Is.EqualTo(3));
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
