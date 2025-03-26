using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MathApp.Dal.Interfaces;
using MathAppApi.Features.Authentication.Dtos;
using MathAppApi.Features.UserExerciseHistory.Controllers;
using MathAppApi.Features.Exercise.Dtos;
using MathAppApi.Features.Exercise.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models;

namespace MathAppApi.Features.Exercise.Services;

public class ExerciseService : IExerciseService
{
    private readonly IUserHistoryEntryRepo _userHistoryEntryRepo;
    private readonly IChapterRepo _chapterRepo;
    private readonly ILessonRepo _lessonRepo;

    private readonly ILogger<ExerciseService> _logger;

    public ExerciseService(IUserHistoryEntryRepo userHistoryEntryRepo, ILogger<ExerciseService> logger, IChapterRepo chapterRepo, ILessonRepo lessonRepo)
    {
        _userHistoryEntryRepo = userHistoryEntryRepo;
        _logger = logger;
        _chapterRepo = chapterRepo;
        _lessonRepo = lessonRepo;
    }

    public async Task<ExerciseResponse> GetExercises(Models.UserProfile userProfile, ExerciseDto dto)
    {
        var chapter = await _chapterRepo.FindOneAsync(e => e.Name == dto.ChapterName);
        if (chapter == null)
        {
            _logger.LogWarning("Chapter not found during exercises fetch attempt.");
            return new ExerciseResponse();
        }

        var subject = chapter.Subjects.First(e => e.Name == dto.SubjectName);
        if (subject == null)
        {
            _logger.LogWarning("Subject not found during exercises fetch attempt.");
            return new ExerciseResponse();
        }

        var lesson = subject.Lessons.First(e => e.Id == dto.LessonId);
        if (lesson == null)
        {
            _logger.LogWarning("Lesson not found during exercises fetch attempt.");
            return new ExerciseResponse();
        }

        var series = lesson.Series.First(e => e.Id == dto.SeriesId);
        if (series == null)
        {
            _logger.LogWarning("Series not found during exercises fetch attempt.");
            return new ExerciseResponse();
        }

        return new ExerciseResponse
        {
            Exercises = series.Exercises.ToList<Models.Exercise>()
        };
    }
}
