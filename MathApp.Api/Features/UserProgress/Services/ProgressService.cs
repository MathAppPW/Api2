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

        var result = new Dictionary<string, ChapterProgressResponseEntry>();

        foreach (var chapter in chapters)
        {
            var subjectsProgress = await GetSubjectsProgressAsync(userProfile, chapter.Name);
            if (subjectsProgress == null)
            {
                _logger.LogWarning($"Couldn't fetch progress for chapter {chapter.Name}.");
                return new ChaptersProgressResponse();
            }

            var subjectsCompleted = 0;
            foreach (var progress in subjectsProgress.Progress)
            {
                if(progress.Value.CurrentLesson > 6)
                {
                    subjectsCompleted++;
                }
            }

            result[chapter.Name] = new ChapterProgressResponseEntry
            {
                SubjectsCompleted = subjectsCompleted,
                SubjectsAll = subjectsProgress.Progress.Count,
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
        var result = new Dictionary<string, SubjectsProgressResponseEntry>();

        foreach (var subject in subjectsList)
        {
            await _subjectRepo.LoadCollectionAsync(subject, e => e.Lessons);

            var currentLesson = 1;//TODO - jesli dodamy wstep teoretyczny to uwzglednic 0
            var seriesCompleted = 0;
            var seriesAll = 0;

            foreach(var lesson in subject.Lessons)
            {
                await _lessonRepo.LoadCollectionAsync(lesson, e => e.Series);
                foreach (var series in lesson.Series)
                {
                    await _seriesRepo.LoadCollectionAsync(series, e => e.Exercises);
                }

                var completed = lesson.Series.Count(series =>
                    series.Exercises.All(e => history.Any(h => h.ExerciseId == e.Id.ToString() && h.SeriesId == series.Id && h.Success))
                );

                if (lesson.Series.Count > 0)
                {
                    if (completed == lesson.Series.Count)
                    {
                        currentLesson++;
                    }
                    else
                    {
                        seriesCompleted = completed;
                        seriesAll = lesson.Series.Count;
                        break;
                    }
                }
            }

            result[subject.Name] = new SubjectsProgressResponseEntry
            {
                CurrentLesson = currentLesson,
                SeriesCompleted = seriesCompleted,
                SeriesAll = seriesAll
            };
        }

        return new SubjectsProgressResponse
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
