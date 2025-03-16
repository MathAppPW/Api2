using MathApp.Dal.Interfaces;
using MathAppApi.Features.UserExerciseHistory.Dtos;
using MathAppApi.Features.UserProfile.Controllers;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace MathAppApi.Features.UserExerciseHistory.Extensions;

public class HistoryUtils
{
    private readonly IUserHistoryEntryRepo _UserHistoryEntryRepo;

    public HistoryUtils(IUserHistoryEntryRepo userHistoryEntryRepo)
    {
        _UserHistoryEntryRepo = userHistoryEntryRepo;
    }

    public async Task<StreakResponse> GetLongestStreak(Models.UserProfile userProfile)
    {
        List<UserHistoryEntry> history = await GetList(userProfile);
        if (history == null || history.Count == 0)
        {
            return new StreakResponse { };
        }

        List<DateTime> successDays = history
            .Where(e => e.Success)
            .Select(e => e.Date.Date)
            .Distinct()
            .OrderBy(date => date)
            .ToList();

        if (successDays.Count == 0)
            return new StreakResponse { };

        int longestStreak = 1;
        int currentStreak = 1;
        DateTime longestStart = DateTime.Today;
        DateTime longestEnd = DateTime.Today;
        DateTime currentStart = DateTime.Today;

        for (int i = 1; i < successDays.Count; i++)
        {
            if ((successDays[i] - successDays[i - 1]).Days == 1)
            {
                currentStreak++;
                longestStreak = Math.Max(longestStreak, currentStreak);
            }
            else
            {
                if (currentStreak > longestStreak)
                {
                    longestStreak = currentStreak;
                    longestStart = currentStart;
                    longestEnd = successDays[i - 1];
                }
                currentStreak = 1;
                currentStart = successDays[i];
            }
        }

        return new StreakResponse
        {
            Streak = longestStreak,
            Start = longestStart,
            End = longestEnd,
        };
    }

    public async Task<StreakResponse> GetCurrentStreak(Models.UserProfile userProfile)
    {
        List<UserHistoryEntry> history = await GetList(userProfile);
        if (history == null || history.Count == 0)
        {
            return new StreakResponse { };
        }

        List<DateTime> successDays = history
            .Where(e => e.Success)
            .Select(e => e.Date.Date)
            .Distinct()
            .OrderBy(date => date)
            .ToList();

        if (successDays.Count == 0)
            return new StreakResponse { };

        DateTime today = DateTime.Today;
        DateTime yesterday = today.AddDays(-1);

        int currentStreak = 0;
        DateTime currentStart = DateTime.Today;

        for (int i = successDays.Count - 1; i >= 0; i--)
        {
            if (successDays[i] == today || successDays[i] == yesterday)
            {
                currentStreak = 1;
                currentStart = successDays[i];
            }
            else if (currentStreak > 0 && successDays[i] == successDays[i + 1].AddDays(-1))
            {
                currentStreak++;
                currentStart = successDays[i];
            }
            else
            {
                break;
            }
        }

        return new StreakResponse
        {
            Streak = currentStreak,
            Start = currentStart,
            End = today
        };
    }

    public async Task<List<TimeSpentDto>> GetTimeSpentPerDay(Models.UserProfile userProfile)
    {
        List<UserHistoryEntry> history = await GetList(userProfile);
        if (history == null || history.Count == 0)
        {
            return [];
        }


        return history
            .GroupBy(e => e.Date.Date)
            .Select(g => new TimeSpentDto
            {
                Date = g.Key,
                SecondsSpent = g.Sum(e => e.TimeSpent)
            })
            .OrderBy(d => d.Date)
            .ToList();
    }

        
    public async Task<List<ExerciseCounterDto>> GetExercisesCountPerDay(Models.UserProfile userProfile)
    {
        List<UserHistoryEntry> history = await GetList(userProfile);
        if (history == null || history.Count == 0)
        {
            return [];
        }

        return history
        .GroupBy(e => e.Date.Date)
        .Select(g => new ExerciseCounterDto
        {
            Date = g.Key,
            Successful = g.Count(e => e.Success),
            Failed = g.Count(e => !e.Success),
            Total = g.Count()
        })
        .OrderBy(dto => dto.Date)
        .ToList();
    }

    public async Task<int> GetExercisesCountAll(Models.UserProfile userProfile)
    {
        List<UserHistoryEntry> history = await GetList(userProfile);
        if (history == null || history.Count == 0)
        {
            return 0;
        }

        return history.Count(e => e.Success);
    }

    private async Task<List<UserHistoryEntry>> GetList(Models.UserProfile userProfile)
    {
        List<UserHistoryEntry> history = [];
        foreach (string entryId in userProfile.History)
        {
            var entry = await _UserHistoryEntryRepo.FindOneAsync(u => u.Id == entryId);
            if (entry == null)
            {
                return [];
            }
            history.Add(entry);
        }

        return history;
    }
}
