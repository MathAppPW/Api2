using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathAppApi.Features.UserExerciseHistory.Dtos;

namespace MathAppApi.Shared.Utils.Interfaces;

public interface IHistoryUtils
{
    public Task<StreakResponse> GetLongestStreak(UserProfile userProfile);
    public Task<StreakResponse> GetCurrentStreak(UserProfile userProfile);
    public Task<List<DateTime>> GetSuccessDays(UserProfile userProfile);
    public Task<List<HistoryGetDaysResponseDay>> GetActivityPerDay(UserProfile userProfile);
    public Task<int> GetExercisesCountAll(UserProfile userProfile);
    public Task<List<UserHistoryEntry>> GetList(UserProfile userProfile);
}
