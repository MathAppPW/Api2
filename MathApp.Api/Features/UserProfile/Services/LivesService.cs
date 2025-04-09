using MathApp.Dal.Interfaces;
using MathAppApi.Features.UserProfile.Controllers;
using MathAppApi.Features.UserProfile.Services.Interfaces;
using Models;

namespace MathAppApi.Features.UserProfile.Services;

public class LivesService : ILivesService
{
    private readonly IUserProfileRepo _userProfileRepo;

    public LivesService(IUserProfileRepo userProfileRepo)
    {
        _userProfileRepo = userProfileRepo;
    }

    public async Task<int> GetSecondsToHeal(Models.UserProfile userProfile)
    {
        await UpdateLives(userProfile);

        if (userProfile.Lives == LivesController._maxLives)
        {
            return 0;
        }
        else
        {
            int timeFromLastUpdate = (int)(DateTime.UtcNow - userProfile.LastLivesUpdate).TotalSeconds;
            int secondsToHeal = LivesController._livesUpdateInterval * 60 - timeFromLastUpdate;
            return secondsToHeal;
        }
    }

    public async Task UpdateLives(Models.UserProfile userProfile)
    {
        var max = LivesController._maxLives;
        var interval = LivesController._livesUpdateInterval;
        var currentTime = DateTime.UtcNow;
        var lastUpdate = userProfile.LastLivesUpdate;
        var timeDifference = currentTime - lastUpdate;
        var livesToAdd = (int)timeDifference.TotalMinutes / interval;

        userProfile.Lives = Math.Min(userProfile.Lives + livesToAdd, max);
        if (userProfile.Lives == max)
        {
            userProfile.LastLivesUpdate = currentTime;
        }
        else
        {
            userProfile.LastLivesUpdate = lastUpdate.AddMinutes(livesToAdd * interval);
        }

        await _userProfileRepo.UpdateAsync(userProfile);
    }
}
