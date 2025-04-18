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
using MathAppApi.Features.UserProfile.Services.Interfaces;

namespace MathApp.Api.Tests.Features.UserProfile.Controllers;


[TestFixture]
public class ExperienceControllerTests
{
    private Mock<IUserProfileRepo> _userRepoMock;
    private Mock<ILogger<ExperienceController>> _loggerMock;
    private Mock<IAchievementsService> _achievementsServiceMock;
    private ExperienceController _controller;

    [SetUp]
    public void SetUp()
    {
        _userRepoMock = new Mock<IUserProfileRepo>();
        _loggerMock = new Mock<ILogger<ExperienceController>>();
        _achievementsServiceMock = new Mock<IAchievementsService>();
        _controller = new ExperienceController(_userRepoMock.Object, _loggerMock.Object, _achievementsServiceMock.Object);

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
    public async Task Add_ShouldReturnOk_WhenExperienceAdded()
    {
        var userProfile = new Models.UserProfile { Id = "123", Experience = 900, Level = 1 };
        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Models.UserProfile, bool>>>())).ReturnsAsync(userProfile);
        _userRepoMock.Setup(repo => repo.UpdateAsync(It.IsAny<Models.UserProfile>())).Returns(Task.CompletedTask);

        var result = await _controller.Add(new ExperienceDto { Amount = 200 });

        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            var response = okResult.Value as ExperienceResponse;
            Assert.That(response, Is.Not.Null);
            if (response != null)
            {
                Assert.That(response.Progress, Is.EqualTo(0.1f));
                Assert.That(response.Level, Is.EqualTo(2));
            }
        });
    }

    [Test]
    public async Task Add_ShouldReturnOk_WhenNoLevelUp()
    {
        var userProfile = new Models.UserProfile { Id = "123", Experience = 950, Level = 1 };
        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Models.UserProfile, bool>>>())).ReturnsAsync(userProfile);
        _userRepoMock.Setup(repo => repo.UpdateAsync(It.IsAny<Models.UserProfile>())).Returns(Task.CompletedTask);

        var result = await _controller.Add(new ExperienceDto { Amount = 40 });

        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            var response = okResult.Value as ExperienceResponse;
            Assert.That(response, Is.Not.Null);
            if (response != null)
            {
                Assert.That(response.Progress, Is.EqualTo(0.99f));
                Assert.That(response.Level, Is.EqualTo(1));
            }
        });
    }

    [Test]
    public async Task Add_ShouldReturnBadRequest_WhenUserNotFound()
    {
        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Models.UserProfile, bool>>>())).ReturnsAsync(default(Models.UserProfile));

        var result = await _controller.Add(new ExperienceDto { Amount = 200 });

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task Add_ShouldReturnUnauthorized_WhenUserIdIsMissing()
    {
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();

        var result = await _controller.Add(new ExperienceDto { Amount = 200 });

        Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
    }

    [Test]
    public async Task Get_ShouldReturnExperience_WhenUserExists()
    {
        var userProfile = new Models.UserProfile { Id = "123", Experience = 1500, Level = 2 };
        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Models.UserProfile, bool>>>())).ReturnsAsync(userProfile);

        var result = await _controller.Get();

        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            var response = okResult.Value as ExperienceResponse;
            Assert.That(response, Is.Not.Null);
            if (response != null)
            {
                Assert.That(response.Progress, Is.EqualTo(0.5f));
                Assert.That(response.Level, Is.EqualTo(2));
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

