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
        List<DateTime> successDays = await GetSuccessDays(userProfile);
        if (successDays.Count == 0)
        {
            return new StreakResponse
            {
                Streak = 0,
                Start = DateTime.Today,
                End = DateTime.Today
            };
        }

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
                if(currentStreak == 2)
                {
                    currentStart = successDays[i - 1];
                }
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
            }
        }

        if (currentStreak > longestStreak)
        {
            longestStreak = currentStreak;
            longestStart = currentStart;
            longestEnd = successDays[successDays.Count - 1];
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
        List<DateTime> successDays = await GetSuccessDays(userProfile);
        if (successDays.Count == 0)
        {
            return new StreakResponse
            {
                Streak = 0,
                Start = DateTime.Today,
                End = DateTime.Today
            };
        }

        DateTime today = DateTime.Today;
        DateTime yesterday = today.AddDays(-1);

        int currentStreak = 0;
        DateTime currentStart = DateTime.Today;

        for (int i = successDays.Count - 1; i >= 0; i--)
        {
            bool isRecentDay = successDays[i] == today || successDays[i] == yesterday;
            bool doesContinueStreak = !isRecentDay && (successDays[i + 1] - successDays[i]).Days == 1;
            if (isRecentDay || (currentStreak > 0 && doesContinueStreak))
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

    private async Task<List<DateTime>> GetSuccessDays(Models.UserProfile userProfile)
    {
        List<UserHistoryEntry> history = await GetList(userProfile);
        if (history == null || history.Count == 0)
        {
            Console.WriteLine("CHUJ");
            return [];
        }

        List<DateTime> successDays = history
            .Where(e => e.Success)
            .Select(e => e.Date.Date)
            .Distinct()
            .OrderBy(date => date)
            .ToList();

        if (successDays.Count == 0)
        {
            return [];
        }

        return successDays;
    }

    public async Task<List<HistoryGetDaysResponseDay>> GetActivityPerDay(Models.UserProfile userProfile)
    {
        List<UserHistoryEntry> history = await GetList(userProfile);
        if (history == null || history.Count == 0)
        {
            return [];
        }

        return history
            .GroupBy(e => e.Date.Date)
            .Select(g => new HistoryGetDaysResponseDay
            {
                Date = g.Key,
                SecondsSpent = g.Sum(e => e.TimeSpent),
                ExercisesCount = g.Count(),
                ExercisesCountSuccessful = g.Count(e => e.Success),
                ExercisesCountFailed = g.Count(e => !e.Success),
            })
            .OrderBy(d => d.Date)
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
