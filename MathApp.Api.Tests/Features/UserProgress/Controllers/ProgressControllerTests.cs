using MathAppApi.Features.UserProgress.Controllers;
using MathAppApi.Features.UserProgress.Dtos;
using MathAppApi.Features.UserProgress.Services.Interfaces;
using MathApp.Dal.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using Models;
using System;
using System.Linq.Expressions;
using MathAppApi.Features.UserExerciseHistory.Controllers;

namespace MathApp.Api.Tests.Features.UserProgress.Controllers;

[TestFixture]
public class ProgressControllerTests
{
    private Mock<IUserProfileRepo> _userRepoMock;
    private Mock<IProgressService> _progressServiceMock;
    private Mock<ILogger<HistoryController>> _loggerMock;
    private ProgressController _controller;

    [SetUp]
    public void SetUp()
    {
        _userRepoMock = new Mock<IUserProfileRepo>();
        _progressServiceMock = new Mock<IProgressService>();
        _loggerMock = new Mock<ILogger<HistoryController>>();

        _controller = new ProgressController(_userRepoMock.Object, _loggerMock.Object, _progressServiceMock.Object);

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
    public async Task GetChapters_ShouldReturnOk_WhenUserExists()
    {
        var userProfile = new Models.UserProfile { Id = "123" };
        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<Expression<Func<Models.UserProfile, bool>>>())).ReturnsAsync(userProfile);
        _progressServiceMock.Setup(service => service.GetChaptersProgressAsync(userProfile)).ReturnsAsync(new ChaptersProgressResponse());

        var result = await _controller.GetChapters();

        Assert.IsInstanceOf<OkObjectResult>(result);
    }

    [Test]
    public async Task GetChapters_ShouldReturnBadRequest_WhenUserNotFound()
    {
        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<Expression<Func<Models.UserProfile, bool>>>())).ReturnsAsync(default(Models.UserProfile));

        var result = await _controller.GetChapters();

        Assert.IsInstanceOf<BadRequestObjectResult>(result);
    }

    [Test]
    public async Task GetChapters_ShouldReturnUnauthorized_WhenUserIdIsMissing()
    {
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();

        var result = await _controller.GetChapters();

        Assert.IsInstanceOf<UnauthorizedResult>(result);
    }

    [Test]
    public async Task GetSubjects_ShouldReturnOk_WhenUserExists()
    {
        var userProfile = new Models.UserProfile { Id = "123" };
        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<Expression<Func<Models.UserProfile, bool>>>())).ReturnsAsync(userProfile);
        _progressServiceMock.Setup(service => service.GetSubjectsProgressAsync(userProfile, "chapter")).ReturnsAsync(new SubjectsProgressResponse());

        var result = await _controller.GetSubjects("chapter");

        Assert.IsInstanceOf<OkObjectResult>(result);
    }

    [Test]
    public async Task GetSubjects_ShouldReturnBadRequest_WhenUserNotFound()
    {
        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<Expression<Func<Models.UserProfile, bool>>>())).ReturnsAsync(default(Models.UserProfile));

        var result = await _controller.GetSubjects("chapter");

        Assert.IsInstanceOf<BadRequestObjectResult>(result);
    }

    [Test]
    public async Task GetSubjects_ShouldReturnUnauthorized_WhenUserIdMissing()
    {
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();

        var result = await _controller.GetSubjects("chapter");

        Assert.IsInstanceOf<UnauthorizedResult>(result);
    }
}
