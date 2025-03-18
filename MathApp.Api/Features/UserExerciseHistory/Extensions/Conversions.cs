using MathAppApi.Features.UserExerciseHistory.Dtos;
using MathAppApi.Features.UserProfile.Controllers;
using MathAppApi.Features.UserProfile.Dtos;
using Models;

namespace MathAppApi.Features.UserExerciseHistory.Extensions;

public static class Conversions
{
    public static UserHistoryEntry ToModel(this HistoryEntryDto historyEntry)
    {
        return new UserHistoryEntry
        {
            Id = Guid.NewGuid().ToString(),
            ExerciseId = historyEntry.ExerciseId,
            Date = historyEntry.Date,
            TimeSpent = historyEntry.TimeSpent,
            Success = historyEntry.Success
        };
    }
}
