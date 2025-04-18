using MathAppApi.Features.UserProgress.Controllers;
using MathAppApi.Features.Exercise.Dtos;
using MathAppApi.Features.Exercise.Services.Interfaces;
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

namespace MathApp.Api.Tests.Features.Exercise.Controllers;

[TestFixture]
public class ExerciseControllerTests
{
    private Mock<IExerciseService> _exerciseServiceMock;
    private Mock<ILogger<HistoryController>> _loggerMock;
    private ExerciseController _controller;

    [SetUp]
    public void SetUp()
    {
        _exerciseServiceMock = new Mock<IExerciseService>();
        _loggerMock = new Mock<ILogger<HistoryController>>();

        _controller = new ExerciseController(_loggerMock.Object, _exerciseServiceMock.Object);

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
    public async Task GetExercises_ShouldReturnExercisesForGivenSeries()
    {
        var exercises = new List<SeriesResponseExercise>
        {
            new SeriesResponseExercise { Id = 1, Contents = "2+2" },
            new SeriesResponseExercise { Id = 2, Contents = "3+5" }
        };

        var response = new ExerciseResponse { Exercises = exercises };

        _exerciseServiceMock.Setup(service => service.GetExercises(2)).ReturnsAsync(response);

        var result = await _controller.GetExercises(2);

        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<OkObjectResult>(result);
            var ok = result as OkObjectResult;
            var value = ok?.Value as ExerciseResponse;
            Assert.That(value, Is.Not.Null);
            Assert.That(value!.Exercises.Count, Is.EqualTo(2));
            Assert.That(value.Exercises[0].Contents, Is.EqualTo("2+2"));
            Assert.That(value.Exercises[1].Contents, Is.EqualTo("3+5"));
        });
    }

    [Test]
    public async Task GetExercises_ShouldReturnEmptyList_WhenNoExercisesInSeries()
    {
        var response = new ExerciseResponse { Exercises = new List<SeriesResponseExercise>() };

        _exerciseServiceMock.Setup(service => service.GetExercises(3)).ReturnsAsync(response);

        var result = await _controller.GetExercises(3);

        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<OkObjectResult>(result);
            var ok = result as OkObjectResult;
            var value = ok?.Value as ExerciseResponse;
            Assert.That(value, Is.Not.Null);
            Assert.That(value!.Exercises.Count, Is.EqualTo(0));
        });
    }

    [Test]
    public async Task GetExercises_ShouldReturnUnauthorized_WhenUserIdIsMissing()
    {
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();

        var result = await _controller.GetExercises(2);

        Assert.IsInstanceOf<UnauthorizedResult>(result);
    }
}
