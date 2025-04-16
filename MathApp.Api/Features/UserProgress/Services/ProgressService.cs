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
    private readonly ISubjectRepo _subjectRepo;
    private readonly ILessonRepo _lessonRepo;
    private readonly ISeriesRepo _seriesRepo;

    private readonly ILogger<ProgressService> _logger;

    public ProgressService(IUserHistoryEntryRepo userHistoryEntryRepo, ILogger<ProgressService> logger, IChapterRepo chapterRepo, ISubjectRepo subjectRepo, ILessonRepo lessonRepo, ISeriesRepo seriesRepo)
    {
        _userHistoryEntryRepo = userHistoryEntryRepo;
        _logger = logger;
        _chapterRepo = chapterRepo;
        _subjectRepo = subjectRepo;
        _lessonRepo = lessonRepo;
        _seriesRepo = seriesRepo;
    }

    public async Task<ChaptersProgressResponse> GetChaptersProgressAsync(Models.UserProfile userProfile)
    {
        var chapters = await _chapterRepo.GetAllAsync();

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

            var fullyCompleted = 0;
            foreach (var progress in subjectsProgress.Progress)
            {
                if(progress.Value.ExercisesCompletedPercent == 1f)
                {
                    fullyCompleted++;
                }
            }

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

    public async Task<SubjectsProgressResponse> GetSubjectsProgressAsync(Models.UserProfile userProfile, string chapterName)
    {
        var history = await GetUserHistory(userProfile);

        var chapter = await _chapterRepo.FindOneAsync(e => e.Name == chapterName);
        if (chapter == null)
        {
            _logger.LogWarning($"Chapter with name {chapterName} not found.");
            return new SubjectsProgressResponse();
        }

        await _chapterRepo.LoadCollectionAsync(chapter, e => e.Subjects);
        var subjectsList = chapter.Subjects;
        var result = new Dictionary<string, ProgressDto>();

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
                sum += progress.Value.ExercisesCompletedPercent;
            }

            var fullyCompleted = 0;
            foreach (var progress in lessonsProgress.Progress)
            {
                if(progress.Value.ExercisesCompletedPercent == 1f)
                {
                    fullyCompleted++;
                }
            }

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

    public async Task<LessonsProgressResponse> GetLessonsProgressAsync(Models.UserProfile userProfile, string chapterName, string subjectName)
    {
        var history = await GetUserHistory(userProfile);

        var chapter = await _chapterRepo.FindOneAsync(e => e.Name == chapterName);
        if (chapter == null)
        {
            _logger.LogWarning($"Chapter with name {chapterName} not found.");
            return new LessonsProgressResponse();
        }

        await _chapterRepo.LoadCollectionAsync(chapter, e => e.Subjects);
        var subjectsList = chapter.Subjects;
        var subject = subjectsList.First(s => s.Name == subjectName);
        if (subject == null)
        {
            _logger.LogWarning($"Subject with name {subjectName} not found.");
            return new LessonsProgressResponse();
        }

        await _subjectRepo.LoadCollectionAsync(subject, e => e.Lessons);
        var lessonsList = subject.Lessons;
        var result = new Dictionary<int, ProgressDto>();

        foreach (var lesson in lessonsList)
        {
            await _lessonRepo.LoadCollectionAsync(lesson, e => e.Series);
            var seriesList = lesson.Series;

            foreach (var series in seriesList)
            {
                await _seriesRepo.LoadCollectionAsync(series, e => e.Exercises);
            }

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
