using MathAppApi.Features.UserExerciseHistory.Controllers;
using MathAppApi.Features.UserExerciseHistory.Dtos;
using MathApp.Dal.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using NUnit.Framework;
using Models;
using System.Collections.Generic;
using System;
using System.Linq.Expressions;
using MathAppApi.Shared.Utils.Interfaces;
using MathAppApi.Features.UserProfile.Services.Interfaces;

namespace MathApp.Api.Tests.Features.UserExerciseHistory.Controllers;

[TestFixture]
public class HistoryControllerTests
{
    private Mock<IUserProfileRepo> _userRepoMock;
    private Mock<IUserHistoryEntryRepo> _historyRepoMock;
    private Mock<ILogger<HistoryController>> _loggerMock;
    private Mock<IHistoryUtils> _historyUtils;
    private Mock<IAchievementsService> _achievementsServiceMock;
    private HistoryController _controller;

    [SetUp]
    public void SetUp()
    {
        _userRepoMock = new Mock<IUserProfileRepo>();
        _historyRepoMock = new Mock<IUserHistoryEntryRepo>();
        _loggerMock = new Mock<ILogger<HistoryController>>();
        _historyUtils = new Mock<IHistoryUtils>();
        _achievementsServiceMock = new Mock<IAchievementsService>();
        _controller = new HistoryController(_userRepoMock.Object, _historyRepoMock.Object, _loggerMock.Object, _historyUtils.Object, _achievementsServiceMock.Object);

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
    public async Task Add_ShouldReturnOk_WhenEntryAdded()
    {
        var userProfile = new Models.UserProfile { Id = "123", History = new List<string>() };
        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Models.UserProfile, bool>>>())).ReturnsAsync(userProfile);
        _historyRepoMock.Setup(repo => repo.AddAsync(It.IsAny<UserHistoryEntry>())).Returns(Task.CompletedTask);
        _userRepoMock.Setup(repo => repo.UpdateAsync(It.IsAny<Models.UserProfile>())).Returns(Task.CompletedTask);

        var dto = new HistoryEntryDto
        {
            SeriesId = 1,
            Date = DateTime.UtcNow,
            TimeSpent = 60,
            SuccessfulCount = 6,
            FailedCount = 2
        };

        var result = await _controller.Add(dto);

        Assert.IsInstanceOf<OkResult>(result);
    }

    [Test]
    public async Task Add_ShouldReturnBadRequest_WhenUserNotFound()
    {
        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Models.UserProfile, bool>>>())).ReturnsAsync(default(Models.UserProfile));

        var dto = new HistoryEntryDto
        {
            SeriesId = 1,
            Date = DateTime.UtcNow,
            TimeSpent = 60,
            SuccessfulCount = 6,
            FailedCount = 2
        };

        var result = await _controller.Add(dto);

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task Add_ShouldReturnUnauthorized_WhenUserIdIsMissing()
    {
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();

        var dto = new HistoryEntryDto
        {
            SeriesId = 1,
            Date = DateTime.UtcNow,
            TimeSpent = 60,
            SuccessfulCount = 6,
            FailedCount = 2
        };

        var result = await _controller.Add(dto);

        Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
    }

    [Test]
    public async Task Get_ShouldReturnEntry_WhenEntryExists()
    {
        var historyEntry = new UserHistoryEntry { Id = "entry1" };
        _historyRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserHistoryEntry, bool>>>())).ReturnsAsync(historyEntry);

        var result = await _controller.Get(new HistoryGetDto { Id = "entry1" });

        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.EqualTo(historyEntry));
        });
    }

    [Test]
    public async Task Get_ShouldReturnBadRequest_WhenEntryNotFound()
    {
        _historyRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserHistoryEntry, bool>>>())).ReturnsAsync(default(UserHistoryEntry));

        var result = await _controller.Get(new HistoryGetDto { Id = "entry1" });

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task GetAll_ShouldReturnEntries_WhenUserExists()
    {
        var userProfile = new Models.UserProfile { Id = "123", History = new List<string> { "entry1", "entry2" } };
        var historyEntry1 = new UserHistoryEntry { Id = "entry1" };
        var historyEntry2 = new UserHistoryEntry { Id = "entry2" };

        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Models.UserProfile, bool>>>())).ReturnsAsync(userProfile);

        _historyRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<Expression<Func<UserHistoryEntry, bool>>>()))
        .ReturnsAsync((Expression<Func<UserHistoryEntry, bool>> predicate) =>
        {
            if (predicate.Compile().Invoke(historyEntry1))
                return historyEntry1;
            if (predicate.Compile().Invoke(historyEntry2))
                return historyEntry2;
            return null;
        });

        var result = await _controller.GetAll();

        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            var response = okResult.Value as HistoryGetAllResponse;
            Assert.That(response, Is.Not.Null);
            Assert.That(response?.Entries.Count, Is.EqualTo(2));
        });
    }

    [Test]
    public async Task GetAll_ShouldReturnBadRequest_WhenUserNotFound()
    {
        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Models.UserProfile, bool>>>())).ReturnsAsync(default(Models.UserProfile));

        var result = await _controller.GetAll();

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task Delete_ShouldReturnOk_WhenEntryDeleted()
    {
        var userProfile = new Models.UserProfile { Id = "123", History = new List<string> { "entry1" } };
        var historyEntry = new UserHistoryEntry { Id = "entry1" };

        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Models.UserProfile, bool>>>())).ReturnsAsync(userProfile);
        _historyRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserHistoryEntry, bool>>>())).ReturnsAsync(historyEntry);
        _historyRepoMock.Setup(repo => repo.RemoveAsync(It.IsAny<UserHistoryEntry>())).Returns(Task.CompletedTask);
        _userRepoMock.Setup(repo => repo.UpdateAsync(It.IsAny<Models.UserProfile>())).Returns(Task.CompletedTask);

        var result = await _controller.Delete("entry1");

        Assert.IsInstanceOf<OkResult>(result);
    }

    [Test]
    public async Task Delete_ShouldReturnBadRequest_WhenEntryNotFound()
    {
        _historyRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserHistoryEntry, bool>>>())).ReturnsAsync(default(UserHistoryEntry));

        var result = await _controller.Delete("entry1");

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task Delete_ShouldReturnUnauthorized_WhenUserIdIsMissing()
    {
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();

        var result = await _controller.Delete("entry1");

        Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
    }
}
