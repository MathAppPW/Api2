using MathApp.Dal.Interfaces;
using MathAppApi.Features.UserProfile.Dtos;
using MathAppApi.Features.UserProfile.Services.Interfaces;
using MathAppApi.Shared.Utils.Interfaces;
using Models;

namespace MathAppApi.Features.UserProfile.Services;

public class AchievementsService : IAchievementsService
{
    private readonly IUserProfileRepo _userProfileRepo;
    private readonly IFriendshipRepo _friendshipRepo;

    private readonly IHistoryUtils _historyUtils;

    public AchievementsService(IUserProfileRepo userProfileRepo, IFriendshipRepo friendshipRepo, IHistoryUtils historyUtils)
    {
        _userProfileRepo = userProfileRepo;
        _friendshipRepo = friendshipRepo;
        _historyUtils = historyUtils;
    }

    public async Task UpdateAchievements(Models.UserProfile userProfile)
    {
        var achievementsRocket = await GetUnlockedRockets(userProfile);
        var achievementsAvatar = await GetUnlockedAvatars(userProfile);

        for(int i = 0; i < achievementsRocket.Count; i++)
        {
            while (i >= userProfile.AchievementsRocket.Count)
            {
                userProfile.AchievementsRocket.Add(false);
            }
            if (achievementsRocket[i])
            {
                userProfile.AchievementsRocket[i] = true;
            }
        }

        for(int i = 0; i < achievementsAvatar.Count; i++)
        {
            while (i >= userProfile.AchievementsAvatar.Count)
            {
                userProfile.AchievementsAvatar.Add(false);
            }
            if (achievementsAvatar[i])
            {
                userProfile.AchievementsAvatar[i] = true;
            }
        }

        await _userProfileRepo.UpdateAsync(userProfile);
    }

    private async Task<List<bool>> GetUnlockedRockets(Models.UserProfile userProfile)
    {
        var achievementsList = new List<bool>();

        achievementsList.Add(true);

        achievementsList.Add(userProfile.Level >= 30);

        var friendships = await _friendshipRepo.FindAllAsync(f => f.UserId1 == userProfile.Id || f.UserId2 == userProfile.Id);
        achievementsList.Add(friendships.Count >= 4);

        var exercisesCompleted = await _historyUtils.GetExercisesCountSuccessful(userProfile);
        achievementsList.Add(exercisesCompleted >= 200);

        var longestStreak = await _historyUtils.GetLongestStreak(userProfile);
        achievementsList.Add(longestStreak.Streak >= 300);

        return achievementsList;
    }

    private async Task<List<bool>> GetUnlockedAvatars(Models.UserProfile userProfile)
    {
        var achievementsList = new List<bool>();

        achievementsList.Add(true);
        achievementsList.Add(true);

        achievementsList.Add(userProfile.Level >= 10);
        achievementsList.Add(userProfile.Level >= 40);

        var friendships = await _friendshipRepo.FindAllAsync(f => f.UserId1 == userProfile.Id || f.UserId2 == userProfile.Id);
        achievementsList.Add(friendships.Count >= 2);

        var exercisesCompleted = await _historyUtils.GetExercisesCountSuccessful(userProfile);
        achievementsList.Add(exercisesCompleted >= 50);
        achievementsList.Add(exercisesCompleted >= 400);

        var longestStreak = await _historyUtils.GetLongestStreak(userProfile);
        achievementsList.Add(longestStreak.Streak >= 100);
        achievementsList.Add(longestStreak.Streak >= 500);

        achievementsList.Add(false);//TODO leaderboard top 3

        return achievementsList;
    }
}
