using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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
    private readonly IChapterRepo _chapterRepo;
    private readonly ILessonRepo _lessonRepo;

    private readonly ILogger<ProgressService> _logger;

    public ProgressService(IUserHistoryEntryRepo userHistoryEntryRepo, ILogger<ProgressService> logger, IChapterRepo chapterRepo, ILessonRepo lessonRepo)
    {
        _userHistoryEntryRepo = userHistoryEntryRepo;
        _logger = logger;
        _chapterRepo = chapterRepo;
        _lessonRepo = lessonRepo;
    }

    public async Task<ChaptersProgressResponse> GetChaptersProgressAsync(Models.UserProfile userProfile)
    {
        var chapters = await _chapterRepo.GetAllAsync();

        var result = new Dictionary<Chapter, float>();

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
                sum += progress.Value;
            }

            result[chapter] = sum / subjectsProgress.Progress.Count;
        }

        return new ChaptersProgressResponse
        {
            Progress = result
        };
    }

    public async Task<SubjectsProgressResponse> GetSubjectsProgressAsync(Models.UserProfile userProfile, string chapterName)
    {
        var history = await GetUserHistory(userProfile);

        var chapter = await _chapterRepo.FindOneAsync(e => e.Name == chapterName);
        if (chapter == null)
        {
            _logger.LogWarning($"Chapter with name {chapterName} not found.");
            return new SubjectsProgressResponse();
        }

        var subjectsList = chapter.Subjects;
        var result = new Dictionary<Subject, float>();

        foreach (var subject in subjectsList)
        {
            var lessonsProgress = await GetLessonsProgressAsync(userProfile, chapter.Name, subject.Name);
            if (lessonsProgress == null)
            {
                _logger.LogWarning($"Couldn't fetch progress for subject {subject.Name}.");
                return new SubjectsProgressResponse();
            }

            var sum = 0f;
            foreach (var progress in lessonsProgress.Progress)
            {
                sum += progress.Value;
            }

            result[subject] = sum / lessonsProgress.Progress.Count;
        }

        return new SubjectsProgressResponse
        {
            Progress = result
        };
    }

    public async Task<LessonsProgressResponse> GetLessonsProgressAsync(Models.UserProfile userProfile, string chapterName, string subjectName)
    {
        var history = await GetUserHistory(userProfile);

        var chapter = await _chapterRepo.FindOneAsync(e => e.Name == chapterName);
        if (chapter == null)
        {
            _logger.LogWarning($"Chapter with name {chapterName} not found.");
            return new LessonsProgressResponse();
        }
        
        var subjectsList = chapter.Subjects;
        var subject = subjectsList.First(s => s.Name == subjectName);
        if (subject == null)
        {
            _logger.LogWarning($"Subject with name {subjectName} not found.");
            return new LessonsProgressResponse();
        }

        var lessonsList = subject.Lessons;
        var result = new Dictionary<Lesson, float>();

        foreach (var lesson in lessonsList)
        {
            var seriesList = lesson.Series;
            var completedSeries = seriesList.Count(series =>
                series.Exercises.All(e => history.Any(h => h.ExerciseId == e.Id.ToString() && h.SeriesId == series.Id && h.Success))
            );

            var progress = seriesList.Count > 0 ? completedSeries / seriesList.Count : 0;
            result[lesson] = progress;
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
