using Microsoft.AspNetCore.Mvc;

namespace MathAppApi.Features.UserProfile.Services.Interfaces;

public interface ILivesService
{
    public Task<int> GetSecondsToHeal(Models.UserProfile userProfile);
    public Task UpdateLives(Models.UserProfile userProfile);
}
