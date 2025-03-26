using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MathApp.Dal.Interfaces;
using MathAppApi.Features.Authentication.Dtos;
using MathAppApi.Features.Progress.Services.Interfaces;
using MathAppApi.Features.UserExerciseHistory.Controllers;
using MathAppApi.Features.UserExerciseHistory.Dtos;
using Microsoft.EntityFrameworkCore;
using Models;

namespace MathAppApi.Features.Progress.Services;

public class ProgressService : IProgressService
{
    private readonly IUserProfileRepo _userProfileRepo;
    private readonly IUserHistoryEntryRepo _userHistoryEntryRepo;
    private readonly IChapterRepo _chapterRepo;
    private readonly ILessonRepo _lessonRepo;
    private readonly IExerciseRepo _exerciseRepo;
    private readonly ISeriesRepo _seriesRepo;

    private readonly ILogger<ProgressService> _logger;

    public ProgressService(IUserProfileRepo userProfileRepo, IUserHistoryEntryRepo userHistoryEntryRepo, ILogger<ProgressService> logger, IChapterRepo chapterRepo, ILessonRepo lessonRepo, IExerciseRepo exerciseRepo, ISeriesRepo seriesRepo)
    {
        _userProfileRepo = userProfileRepo;
        _userHistoryEntryRepo = userHistoryEntryRepo;
        _logger = logger;
        _chapterRepo = chapterRepo;
        _lessonRepo = lessonRepo;
        _exerciseRepo = exerciseRepo;
        _seriesRepo = seriesRepo;
    }

    public async Task<AllProgressDto> GetAllProgressAsync(Models.UserProfile userProfile)
    {
        var chapters = await _chapterRepo.GetAllAsync();

        var result = new AllProgressDto
        {
            Chapters = new List<ChapterProgressDto>()
        };

        foreach (var chapter in chapters)
        {
            result.Chapters.Add(await GetChapterProgressAsync(userProfile, chapter.Name));
        }

        return result;
    }

    public async Task<ChapterProgressDto> GetChapterProgressAsync(Models.UserProfile userProfile, string chapterName)
    {
        var history = await GetUserHistory(userProfile);

        var chapter = await _chapterRepo.FindOneAsync(e => e.Name == chapterName);
        if (chapter == null)
        {
            _logger.LogWarning($"Chapter with name {chapterName} not found.");
            return new ChapterProgressDto();
        }
        var lessonsList = chapter.Lessons;
        float progressSum = 0f;

        foreach (var lesson in lessonsList)
        {
            var lessonProgress = await GetLessonProgressAsync(userProfile, lesson.Id);
            progressSum += lessonProgress.Progress;
        }

        var progress = progressSum / lessonsList.Count;

        return new ChapterProgressDto
        {
            ChapterName = chapter.Name,
            Progress = MathF.Round(progress, 2)
        };
    }

    public async Task<LessonProgressDto> GetLessonProgressAsync(Models.UserProfile userProfile, int lessonId)
    {
        var history = await GetUserHistory(userProfile);

        var lesson = await _lessonRepo.FindOneAsync(e => e.Id == lessonId);
        if (lesson == null)
        {
            _logger.LogWarning($"Lesson with ID {lessonId} not found.");
            return new LessonProgressDto();
        }

        var seriesList = lesson.Series;
        var completedSeries = seriesList.Count(series =>
            series.Exercises.All(e => history.Any(h => h.ExerciseId == e.Id.ToString() && h.Success))
        );

        var progress = seriesList.Count > 0 ? completedSeries / seriesList.Count : 0;

        return new LessonProgressDto
        {
            LessonId = lesson.Id,
            Progress = MathF.Round(progress, 2)
        };
    }

    private async Task<List<UserHistoryEntry>> GetUserHistory(Models.UserProfile userProfile)
    {
        List<UserHistoryEntry> history = [];
        foreach (string entryId in userProfile.History)
        {
            var entry = await _userHistoryEntryRepo.FindOneAsync(u => u.Id == entryId);
            if (entry == null)
            {
                _logger.LogWarning("User history entry not found during fetch attempt.");
                return [];
            }
            history.Add(entry);
        }

        return history;
    }
}
