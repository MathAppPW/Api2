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
public class ProfileSkinControllerTests
{
    private Mock<IUserProfileRepo> _userRepoMock;
    private Mock<ILogger<ProfileSkinController>> _loggerMock;
    private ProfileSkinController _controller;

    [SetUp]
    public void SetUp()
    {
        _userRepoMock = new Mock<IUserProfileRepo>();
        _loggerMock = new Mock<ILogger<ProfileSkinController>>();
        _controller = new ProfileSkinController(_userRepoMock.Object, _loggerMock.Object);

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
        var userProfile = new Models.UserProfile { Id = "123", ProfileSkin = 1 };
        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Models.UserProfile, bool>>>())).ReturnsAsync(userProfile);
        _userRepoMock.Setup(repo => repo.UpdateAsync(It.IsAny<Models.UserProfile>())).Returns(Task.CompletedTask);

        var result = await _controller.Post(new SkinDto { SkinId = 2 });

        Assert.IsInstanceOf<OkResult>(result);
        Assert.That(userProfile.ProfileSkin, Is.EqualTo(2));
    }

    [Test]
    public async Task Post_ShouldReturnBadRequest_WhenInvalidSkinId()
    {
        var result = await _controller.Post(new SkinDto { SkinId = 20 });

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
        var userProfile = new Models.UserProfile { Id = "123", ProfileSkin = 5 };
        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Models.UserProfile, bool>>>())).ReturnsAsync(userProfile);

        var result = await _controller.Get();

        Assert.IsInstanceOf<OkObjectResult>(result);
        var okResult = (OkObjectResult)result;
        var response = okResult.Value as SkinResponse;
        Assert.That(response, Is.Not.Null);
        if (response != null)
        {
            Assert.That(response.SkinId, Is.EqualTo(5));
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
