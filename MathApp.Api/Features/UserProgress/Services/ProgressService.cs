using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Dal;
using MathApp.Dal.Interfaces;
using MathAppApi.Features.Authentication.Dtos;
using MathAppApi.Features.UserExerciseHistory.Controllers;
using MathAppApi.Features.UserProgress.Dtos;
using MathAppApi.Features.UserProgress.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Models;

namespace MathAppApi.Features.UserProgress.Services;

public class ProgressService : IProgressService
{
    private readonly IUserHistoryEntryRepo _userHistoryEntryRepo;
    private readonly MathAppDbContext _db;

    private readonly ILogger<ProgressService> _logger;

    public ProgressService(IUserHistoryEntryRepo userHistoryEntryRepo, ILogger<ProgressService> logger,
        MathAppDbContext db)
    {
        _userHistoryEntryRepo = userHistoryEntryRepo;
        _logger = logger;
        _db = db;
    }

    public async Task<ChaptersProgressResponse?> GetChaptersProgressAsync(Models.UserProfile userProfile)
    {
        var chapters = await _db.Chapters.ToListAsync();
        var result = new Dictionary<string, ProgressDto>();
        foreach (var chapter in chapters)
        {
            var subjectsProgress = await GetSubjectsProgressAsync(userProfile, chapter.Name);
            if (subjectsProgress == null)
            {
                _logger.LogWarning($"Couldn't fetch progress for chapter {chapter.Name}.");
                return new ChaptersProgressResponse();
            }

            var sum = 0f;
            foreach (var progress in subjectsProgress.Progress)
            {
                sum += progress.Value.ExercisesCompletedPercent;
            }

            var fullyCompleted = subjectsProgress.Progress
                .Count(progress => progress.Value.ExercisesCompletedPercent == 1f);

            var allProgress = subjectsProgress.Progress.Count;
            var percent = allProgress == 0f ? 0f : sum / allProgress;
            result[chapter.Name] = new ProgressDto
            {
                Completed = fullyCompleted,
                All = subjectsProgress.Progress.Count,
                ExercisesCompletedPercent = percent
            };
        }

        return new ChaptersProgressResponse
        {
            Progress = result
        };
    }

    public async Task<SubjectsProgressResponse?> GetSubjectsProgressAsync(Models.UserProfile userProfile, string chapterName)
    {
        var chapter = await _db.Chapters
            .Include(c => c.Subjects)
            .ThenInclude(s => s.Lessons)
            .ThenInclude(l => l.Series)
            .ThenInclude(e => e.Exercises)
            .FirstOrDefaultAsync(c => c.Name == chapterName);
        if (chapter == null)
        {
            _logger.LogWarning($"Chapter with name {chapterName} not found.");
            return new SubjectsProgressResponse();
        }

        var subjectsList = chapter.Subjects;
        var result = new Dictionary<string, ProgressDto>();

        foreach (var subject in subjectsList)
        {
            var lessonsProgress = await GetLessonsProgressAsync(userProfile, subject);
            var sum = 0f;
            foreach (var progress in lessonsProgress!.Progress)
                sum += progress.Value.ExercisesCompletedPercent;

            var fullyCompleted = lessonsProgress.Progress.Count(progress => progress.Value.ExercisesCompletedPercent == 1);

            var allProgress = lessonsProgress.Progress.Count;
            var percent = allProgress == 0f ? 0f : sum / allProgress;
            result[subject.Name] = new ProgressDto
            {
                Completed = fullyCompleted,
                All = lessonsProgress.Progress.Count,
                ExercisesCompletedPercent = percent
            };
        }

        return new SubjectsProgressResponse
        {
            Progress = result
        };
    }

    public async Task<LessonsProgressResponse?> GetLessonsProgressAsync(Models.UserProfile userProfile,
        string subjectName)
    {
        var subject = await _db.Subjects.Include(s => s.Lessons).ThenInclude(l => l.Series).ThenInclude(s => s.Exercises)
            .FirstOrDefaultAsync(s => s.Name == subjectName);
        if (subject == null)
            return null;
        return await GetLessonsProgressAsync(userProfile, subject);
    }

    private async Task<LessonsProgressResponse?> GetLessonsProgressAsync(Models.UserProfile userProfile, Subject subject)
    {
        var history = await GetUserHistory(userProfile);
        var lessonsList = subject.Lessons;
        var result = new Dictionary<int, ProgressDto>();

        foreach (var lesson in lessonsList)
        {
            var seriesList = lesson.Series;
            var completedSeries = seriesList.Count(series =>
                series.Exercises.All(e => history.Any(h => h.ExerciseId == e.Id.ToString() && h.SeriesId == series.Id && h.Success))
            );

            var percent = seriesList.Count > 0 ? completedSeries / seriesList.Count : 0;
            result[lesson.Id] = new ProgressDto
            {
                Completed = completedSeries,
                All = seriesList.Count,
                ExercisesCompletedPercent = percent
            };
        }

        return new LessonsProgressResponse
        {
            Progress = result
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
