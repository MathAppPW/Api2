﻿using Dal;
using MathApp.Dal.Interfaces;
using MathAppApi.Features.Authentication.Dtos;
using MathAppApi.Features.Friends.Dtos;
using MathAppApi.Features.UserProfile.Controllers;
using MathAppApi.Features.UserProfile.Dtos;
using MathAppApi.Shared.Utils.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Security.Claims;

namespace MathAppApi.Features.Friends.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class SearchController : ControllerBase
{
    private readonly IUserProfileRepo _userProfileRepo;
    private readonly IUserRepo _userRepo;
    private readonly IFriendshipRepo _friendshipRepo;

    private readonly ILogger<SearchController> _logger;

    private readonly IHistoryUtils _utils;

    public SearchController(IUserProfileRepo userProfileRepo, IUserRepo userRepo, IFriendshipRepo friendshipRepo, ILogger<SearchController> logger, IHistoryUtils historyUtils)
    {
        _userProfileRepo = userProfileRepo;
        _userRepo = userRepo;
        _friendshipRepo = friendshipRepo;
        _logger = logger;
        _utils = historyUtils;
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<UserSearchResponse>(StatusCodes.Status200OK)]
    [HttpGet("search/{username}")]
    public async Task<IActionResult> SearchShort(string username)
    {
        var userId = User.FindFirst("sub")?.Value;
        if (userId == null)
        {
            _logger.LogInformation("User search attempt with no userId.");
            return Unauthorized();
        }

        var user = await _userRepo.FindOneAsync(u => u.Username == username);
        if (user == null)
        {
            _logger.LogInformation("User not found during user search attempt.");
            return BadRequest(new MessageResponse("User not found"));
        }

        var userProfile = await _userProfileRepo.FindOneAsync(u => u.Id == user.Id);
        if (userProfile == null)
        {
            _logger.LogInformation("User profile not found during user search attempt.");
            return BadRequest(new MessageResponse("User profile not found"));
        }

        return Ok(new UserSearchResponse
        {
            Level = userProfile.Level,
            ProfileSkin = userProfile.ProfileSkin,
            RocketSkin = userProfile.RocketSkin,
            Streak = userProfile.Streak,
        });
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProfileSearchResponse>(StatusCodes.Status200OK)]
    [HttpGet("search-verbose/{username}")]
    public async Task<IActionResult> SearchVerbose(string username)
    {
        var userId = User.FindFirst("sub")?.Value;
        if (userId == null)
        {
            _logger.LogInformation("User search attempt with no userId.");
            return Unauthorized();
        }

        var user = await _userRepo.FindOneAsync(u => u.Username == username);
        if (user == null)
        {
            _logger.LogInformation("User not found during user search attempt.");
            return BadRequest(new MessageResponse("User not found"));
        }

        var userProfile = await _userProfileRepo.FindOneAsync(u => u.Id == user.Id);
        if (userProfile == null)
        {
            _logger.LogInformation("User profile not found during user search attempt.");
            return BadRequest(new MessageResponse("User profile not found"));
        }

        var longestStreak = await _utils.GetLongestStreak(userProfile);
        var exercisesCompleted = await _utils.GetExercisesCountSuccessful(userProfile);

        var friendUser = await _userRepo.FindOneAsync(u => u.Username == username);
        if (friendUser == null)
            return NotFound();

        var friendship = await _friendshipRepo.FindOneAsync(fr =>
            fr.UserId1 == userId && fr.UserId2 == friendUser.Id || fr.UserId1 == friendUser.Id && fr.UserId2 == userId);

        var isFriend = false;
        if(friendship != null)
        {
            isFriend = true;
        }

        return Ok(new ProfileSearchResponse
        {
            Level = userProfile.Level,
            ProfileSkin = userProfile.ProfileSkin,
            RocketSkin = userProfile.RocketSkin,
            Streak = userProfile.Streak,
            MaxStreak = longestStreak.Streak,
            ExercisesCompleted = exercisesCompleted,
            IsFriend = isFriend
        });
    }
}
