using MathAppApi.Features.UserExerciseHistory.Controllers;
using MathAppApi.Features.UserExerciseHistory.Dtos;
using MathApp.Dal.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Security.Claims;
using System.Threading.Tasks;
using Models;
using MathAppApi.Features.UserExerciseHistory.Extensions;
using MathAppApi.Shared.Utils.Interfaces;

namespace MathApp.Api.Tests.Features.UserExerciseHistory.Controllers;

[TestFixture]
public class StreakControllerTests
{
    private Mock<IUserProfileRepo> _userRepoMock;
    private Mock<ILogger<StreakController>> _loggerMock;
    private Mock<ILogger<HistoryController>> _historyLoggerMock;
    private Mock<IUserHistoryEntryRepo> _historyRepoMock;
    private Mock<IHistoryUtils> _historyUtilsMock;
    private List<UserHistoryEntry> _historyEntries;
    private StreakController _controller;
    private HistoryController _historyController;

    [SetUp]
    public void SetUp()
    {
        _userRepoMock = new Mock<IUserProfileRepo>();
        _historyRepoMock = new Mock<IUserHistoryEntryRepo>();
        _loggerMock = new Mock<ILogger<StreakController>>();
        _historyLoggerMock = new Mock<ILogger<HistoryController>>();
        _historyUtilsMock = new Mock<IHistoryUtils>();
        _historyEntries = new List<UserHistoryEntry>();

        _controller = new StreakController(_userRepoMock.Object, _loggerMock.Object, _historyUtilsMock.Object);
        _historyController = new HistoryController(_userRepoMock.Object, _historyRepoMock.Object, _historyLoggerMock.Object, _historyUtilsMock.Object);

        _historyRepoMock.Setup(repo => repo.AddAsync(It.IsAny<UserHistoryEntry>()))
            .Callback<UserHistoryEntry>(entry => _historyEntries.Add(entry))
            .Returns(Task.CompletedTask);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] {
            new Claim(ClaimTypes.NameIdentifier, "123")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
        _historyController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Test]
    public async Task GetLongest_ShouldReturnOk_WhenUserExists()
    {
        var userProfile = new Models.UserProfile { Id = "123" };
        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Models.UserProfile, bool>>>())).ReturnsAsync(userProfile);

        var historyEntries = new List<UserHistoryEntry>();

        _historyRepoMock.Setup(repo => repo.AddAsync(It.IsAny<UserHistoryEntry>()))
            .Callback<UserHistoryEntry>(entry => historyEntries.Add(entry))
            .Returns(Task.CompletedTask);

        _historyRepoMock.Setup(repo => repo.FindOneAsync
            (It.IsAny<System.Linq.Expressions.Expression<System.Func<UserHistoryEntry, bool>>>()))
            .ReturnsAsync((System.Linq.Expressions.Expression<System.Func<UserHistoryEntry, bool>> predicate) =>
            {
                return historyEntries.FirstOrDefault(predicate.Compile());
            });

        _historyUtilsMock.Setup(h => h.GetLongestStreak(It.IsAny<Models.UserProfile>()))
            .ReturnsAsync(new StreakResponse { Streak = 5, Start = DateTime.Today.AddDays(-14), End = DateTime.Today.AddDays(-10) });


        _historyRepoMock.Setup(repo => repo.FindAllAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<UserHistoryEntry, bool>>>()))
            .ReturnsAsync(historyEntries);

        int[] days = { 0, 1, 2, 3, 10, 11, 12, 13, 14, 16 };
        for (int i = 0; i < 10; i++)
        {
            HistoryEntryDto entry = new HistoryEntryDto
            {
                ExerciseId = "1",
                Date = DateTime.Today.AddDays(-days[i]),
                TimeSpent = 10,
                Success = true
            };

            await _historyController.Add(entry);
        }

        int[] failedDays = { 4, 15 };
        for (int i = 0; i < 2; i++)
        {
            HistoryEntryDto entry = new HistoryEntryDto
            {
                ExerciseId = "1",
                Date = DateTime.Today.AddDays(-failedDays[i]),
                TimeSpent = 10,
                Success = false
            };

            await _historyController.Add(entry);
        }

        var result = await _controller.GetLongest();

        Assert.IsInstanceOf<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var response = okResult.Value as StreakResponse;
        Assert.That(response, Is.Not.Null);

        Assert.That(response.Streak, Is.EqualTo(5));
        Assert.That(response.Start, Is.EqualTo(DateTime.Today.AddDays(-14)));
        Assert.That(response.End, Is.EqualTo(DateTime.Today.AddDays(-10)));
    }

    [Test]
    public async Task GetLongest_ShouldReturnBadRequest_WhenUserNotFound()
    {
        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Models.UserProfile, bool>>>())).ReturnsAsync(default(Models.UserProfile));

        var result = await _controller.GetLongest();

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task GetLongest_ShouldReturnUnauthorized_WhenUserIdIsMissing()
    {
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();

        var result = await _controller.GetLongest();

        Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
    }

    [Test]
    public async Task GetCurrent_ShouldReturnOk_WhenUserExists()
    {
        var userProfile = new Models.UserProfile { Id = "123" };
        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Models.UserProfile, bool>>>())).ReturnsAsync(userProfile);

        _historyUtilsMock.Setup(h => h.GetCurrentStreak(It.IsAny<Models.UserProfile>()))
            .ReturnsAsync(new StreakResponse { Streak = 4, Start = DateTime.Today.AddDays(-3), End = DateTime.Today });

        var historyEntries = new List<UserHistoryEntry>();

        _historyRepoMock.Setup(repo => repo.AddAsync(It.IsAny<UserHistoryEntry>()))
            .Callback<UserHistoryEntry>(entry => historyEntries.Add(entry))
            .Returns(Task.CompletedTask);

        _historyRepoMock.Setup(repo => repo.FindOneAsync
            (It.IsAny<System.Linq.Expressions.Expression<System.Func<UserHistoryEntry, bool>>>()))
            .ReturnsAsync((System.Linq.Expressions.Expression<System.Func<UserHistoryEntry, bool>> predicate) =>
            {
                return historyEntries.FirstOrDefault(predicate.Compile());
            });

        _historyRepoMock.Setup(repo => repo.FindAllAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<UserHistoryEntry, bool>>>()))
            .ReturnsAsync(historyEntries);

        int[] days = { 0, 1, 2, 3, 10, 11, 12, 13, 14, 16 };
        for (int i = 0; i < 10; i++)
        {
            HistoryEntryDto entry = new HistoryEntryDto
            {
                ExerciseId = "1",
                Date = DateTime.Today.AddDays(-days[i]),
                TimeSpent = 10,
                Success = true
            };

            await _historyController.Add(entry);
        }

        int[] failedDays = { 4, 15 };
        for (int i = 0; i < 2; i++)
        {
            HistoryEntryDto entry = new HistoryEntryDto
            {
                ExerciseId = "1",
                Date = DateTime.Today.AddDays(-failedDays[i]),
                TimeSpent = 10,
                Success = false
            };

            await _historyController.Add(entry);
        }

        var result = await _controller.GetCurrent();

        Assert.IsInstanceOf<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Console.WriteLine(okResult.Value == null);

        var response = okResult.Value as StreakResponse;
        Assert.That(response, Is.Not.Null);

        Assert.That(response.Streak, Is.EqualTo(4));
        Assert.That(response.Start, Is.EqualTo(DateTime.Today.AddDays(-3)));
        Assert.That(response.End, Is.EqualTo(DateTime.Today));
    }

    [Test]
    public async Task GetCurrent_ShouldReturnBadRequest_WhenUserNotFound()
    {
        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Models.UserProfile, bool>>>())).ReturnsAsync(default(Models.UserProfile));

        var result = await _controller.GetCurrent();

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task GetCurrent_ShouldReturnUnauthorized_WhenUserIdIsMissing()
    {
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();

        var result = await _controller.GetCurrent();

        Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
    }
}
