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
            new Claim(ClaimTypes.NameIdentifier, "123")
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

        var result = await _controller.GetSubjects(new SubjectsProgressDto { ChapterName = "chapter" });

        Assert.IsInstanceOf<OkObjectResult>(result);
    }

    [Test]
    public async Task GetLessons_ShouldReturnOk_WhenUserExists()
    {
        var userProfile = new Models.UserProfile { Id = "123" };
        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<Expression<Func<Models.UserProfile, bool>>>())).ReturnsAsync(userProfile);
        _progressServiceMock.Setup(service => service.GetLessonsProgressAsync(userProfile, "chapter", "subject")).ReturnsAsync(new LessonsProgressResponse());

        var result = await _controller.GetLessons(new LessonsProgressDto { ChapterName = "chapter", SubjectName = "subject" });

        Assert.IsInstanceOf<OkObjectResult>(result);
    }

    [Test]
    public async Task GetSubjects_ShouldReturnBadRequest_WhenUserNotFound()
    {
        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<Expression<Func<Models.UserProfile, bool>>>())).ReturnsAsync(default(Models.UserProfile));

        var result = await _controller.GetSubjects(new SubjectsProgressDto { ChapterName = "chapter" });

        Assert.IsInstanceOf<BadRequestObjectResult>(result);
    }

    [Test]
    public async Task GetLessons_ShouldReturnBadRequest_WhenUserNotFound()
    {
        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<Expression<Func<Models.UserProfile, bool>>>())).ReturnsAsync(default(Models.UserProfile));

        var result = await _controller.GetLessons(new LessonsProgressDto { ChapterName = "chapter", SubjectName = "subject" });

        Assert.IsInstanceOf<BadRequestObjectResult>(result);
    }

    [Test]
    public async Task GetSubjects_ShouldReturnUnauthorized_WhenUserIdMissing()
    {
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();

        var result = await _controller.GetSubjects(new SubjectsProgressDto { ChapterName = "chapter" });

        Assert.IsInstanceOf<UnauthorizedResult>(result);
    }

    [Test]
    public async Task GetLessons_ShouldReturnUnauthorized_WhenUserIdMissing()
    {
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();

        var result = await _controller.GetLessons(new LessonsProgressDto { ChapterName = "chapter", SubjectName = "subject" });

        Assert.IsInstanceOf<UnauthorizedResult>(result);
    }

    /*
    [Test]
    public async Task GetChapters_ShouldReturnProgressForAddedChapters()
    {
        var userProfile = new Models.UserProfile { Id = "123" };
        var chapters = new List<Chapter>
        {
            new Chapter { Name = "Chapter1" },
            new Chapter { Name = "Chapter2" },
            new Chapter { Name = "Chapter3" }
        };

        var response = new ChaptersProgressResponse
        {
            Progress = new Dictionary<Chapter, float>
            {
                { chapters[0], 0.75f },
                { chapters[1], 0.5f },
                { chapters[2], 1.0f }
            }
        };

        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<Expression<Func<Models.UserProfile, bool>>>())).ReturnsAsync(userProfile);
        _progressServiceMock.Setup(service => service.GetChaptersProgressAsync(userProfile)).ReturnsAsync(response);

        var result = await _controller.GetChapters();

        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var value = okResult?.Value as ChaptersProgressResponse;
            Assert.That(value, Is.Not.Null);
            Assert.That(value!.Progress.Count, Is.EqualTo(3));
            Assert.That(value.Progress[chapters[0]], Is.EqualTo(0.75f));
            Assert.That(value.Progress[chapters[1]], Is.EqualTo(0.5f));
            Assert.That(value.Progress[chapters[2]], Is.EqualTo(1.0f));
        });
    }
    
    [Test]
    public async Task GetSubjects_ShouldReturnProgressForMultipleSubjects()
    {
        var userProfile = new Models.UserProfile { Id = "123" };
        var chapterName = "ChapterX";

        var subject1 = new Subject { Name = "SubjectA", ChapterName = chapterName };
        var subject2 = new Subject { Name = "SubjectB", ChapterName = chapterName };

        var response = new SubjectsProgressResponse
        {
            Progress = new Dictionary<Subject, float>
            {
                { subject1, 0.3f },
                { subject2, 0.8f }
            }
        };

        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<Expression<Func<Models.UserProfile, bool>>>())).ReturnsAsync(userProfile);
        _progressServiceMock.Setup(service => service.GetSubjectsProgressAsync(userProfile, chapterName)).ReturnsAsync(response);

        var result = await _controller.GetSubjects(new SubjectsProgressDto { ChapterName = chapterName });

        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var value = okResult?.Value as SubjectsProgressResponse;
            Assert.That(value, Is.Not.Null);
            Assert.That(value!.Progress.Count, Is.EqualTo(2));
            Assert.That(value.Progress[subject1], Is.EqualTo(0.3f));
            Assert.That(value.Progress[subject2], Is.EqualTo(0.8f));
        });
    }
    
    [Test]
    public async Task GetLessons_ShouldReturnProgressForMultipleLessons()
    {
        var userProfile = new Models.UserProfile { Id = "123" };
        var chapterName = "ChapterY";
        var subjectName = "SubjectY";

        var lesson1 = new Lesson { Id = 1, SubjectName = subjectName };
        var lesson2 = new Lesson { Id = 2, SubjectName = subjectName };

        var response = new LessonsProgressResponse
        {
            Progress = new Dictionary<Lesson, float>
            {
                { lesson1, 0.4f },
                { lesson2, 1.0f }
            }
        };

        _userRepoMock.Setup(repo => repo.FindOneAsync(It.IsAny<Expression<Func<Models.UserProfile, bool>>>())).ReturnsAsync(userProfile);
        _progressServiceMock.Setup(service => service.GetLessonsProgressAsync(userProfile, chapterName, subjectName)).ReturnsAsync(response);

        var result = await _controller.GetLessons(new LessonsProgressDto { ChapterName = chapterName, SubjectName = subjectName });

        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var value = okResult?.Value as LessonsProgressResponse;
            Assert.That(value, Is.Not.Null);
            Assert.That(value!.Progress.Count, Is.EqualTo(2));
            Assert.That(value.Progress[lesson1], Is.EqualTo(0.4f));
            Assert.That(value.Progress[lesson2], Is.EqualTo(1.0f));
        });
    }
    */
}
