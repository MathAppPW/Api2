using MathAppApi.Features.UserExerciseHistory.Dtos;
using MathAppApi.Features.UserProfile.Controllers;
using MathAppApi.Features.UserProfile.Dtos;
using MathAppApi.Features.UserProfile.Services.Interfaces;
using Models;

namespace MathAppApi.Features.UserProfile.Extensions;

public static class Conversions
{
    public async static Task<UserProfileResponse> ToDto(this Models.UserProfile userProfile, ILivesService livesService)
    {
        if (userProfile.User == null)
            throw new InvalidOperationException("User has not been loaded!");
        
        return new UserProfileResponse
        {
            Username = userProfile.User.Username,
            Level = userProfile.Level,
            Experience = userProfile.Experience,
            Streak = userProfile.Streak,
            Lives = userProfile.Lives,
            LastLivesUpdate = userProfile.LastLivesUpdate,
            SecondsToHeal = await livesService.GetSecondsToHeal(userProfile),
            RocketSkin = userProfile.RocketSkin,
            ProfileSkin = userProfile.ProfileSkin
        };
    }
}
