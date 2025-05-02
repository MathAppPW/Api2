using Dal;
using MathApp.Dal.Interfaces;
using MathAppApi.Features.Friends.Dto;
using MathAppApi.Features.Friends.Dtos;
using MathAppApi.Features.UserProfile.Services.Interfaces;
using MathAppApi.Shared.Utils.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace MathAppApi.Features.Friends.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class FriendsController : ControllerBase
{
    private readonly IUserRepo _userRepo;
    private readonly IFriendshipRepo _friendshipRepo;
    private readonly IFriendRequestRepo _friendRequestRepo;
    private readonly IUserProfileRepo _userProfileRepo;
    private readonly IHistoryUtils _historyUtils;
    private readonly ILogger<FriendsController> _logger;
    private readonly IAchievementsService _achievementsService;

    public FriendsController(IUserRepo userRepo, IFriendshipRepo friendshipRepo, IFriendRequestRepo friendRequestRepo,
        IUserProfileRepo userProfileRepo, IHistoryUtils historyUtils, ILogger<FriendsController> logger, IAchievementsService achievementsService)
    {
        _userRepo = userRepo;
        _historyUtils = historyUtils;
        _friendshipRepo = friendshipRepo;
        _friendRequestRepo = friendRequestRepo;
        _userProfileRepo = userProfileRepo;
        _logger = logger;
        _achievementsService = achievementsService;
    }

    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPost("new/{friendUsername}")]
    public async Task<IActionResult> SendFriendRequest([FromRoute] string friendUsername)
    {
        var senderId = User.FindFirst("sub")?.Value;
        if (senderId == null)
            return Unauthorized();
        var sender = await _userRepo.GetAsync(senderId);
        var receiver = await _userRepo.FindOneAsync(u => u.Username == friendUsername);
        if (sender == null || receiver == null || senderId == receiver.Id)
            return BadRequest();

        var isDuplicate = await _friendshipRepo.AnyAsync(f =>
            f.UserId1 == senderId && f.UserId2 == receiver.Id || f.UserId2 == senderId && f.UserId1 == receiver.Id);
        if (isDuplicate)
            return BadRequest("Friendship already exists");

        var request = new FriendRequest()
        {
            ReceiverUserId = receiver.Id,
            SenderUserId = sender.Id,
            TimeStamp = DateTime.UtcNow,
        };
        await _friendRequestRepo.AddAsync(request);
        return Ok();
    }

    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<List<FriendRequestDto>>(StatusCodes.Status200OK)]
    [HttpGet("getPendingRequests")]
    public async Task<IActionResult> GetPendingRequests()
    {
        var userId = User.FindFirst("sub")?.Value;
        if (userId == null)
            return Unauthorized();
        var receiver = await _userRepo.GetAsync(userId);
        if (receiver == null)
            return Unauthorized();

        var requests = await _friendRequestRepo.FindAllAsync(fr => fr.ReceiverUserId == userId);

        var dtoTasks = requests.Select(async r =>
        {
            var profile = await _userProfileRepo.FindOneAsync(u => u.User!.Id == r.SenderUserId);
            return new FriendRequestDto
            {
                SenderName = r.Sender!.Username,
                ReceiverName = receiver.Username,
                TimeStamp = r.TimeStamp,
                Id = r.Id,
                AvatarSkinId = profile!.ProfileSkin
            };
        });
        var dtos = await Task.WhenAll(dtoTasks);

        return Ok(dtos);
    }

    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPost("respond")]
    public async Task<IActionResult> RespondToFriendRequest([FromBody] FriendRequestResponse response)
    {
        var request = await _friendRequestRepo.GetAsync(response.RequestId);
        if (request == null)
            return NotFound();
        var userId = User.FindFirst("sub")?.Value;
        if (userId == null || request.ReceiverUserId != userId)
            return Forbid();
        if (response.DidAccept)
        {
            var friendship = new Friendship()
            {
                UserId1 = request.SenderUserId,
                UserId2 = request.ReceiverUserId
            };
            await _friendshipRepo.AddAsync(friendship);

            var userProfile = await _userProfileRepo.FindOneAsync(up => up.Id == request.SenderUserId);
            if (userProfile == null)
                return NotFound();
            await _achievementsService.UpdateAchievements(userProfile);
        }
        else
            await _friendRequestRepo.RemoveAsync(request);

        return Ok();
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("removeFriend/{username}")]
    public async Task<IActionResult> RemoveFriend([FromRoute] string username)
    {
        var userId = User.FindFirst("sub")?.Value;
        if (userId == null)
            return Unauthorized();
        var user = await _userRepo.FindOneAsync(u => u.Username == username);
        if (user == null)
            return NotFound();
        var friendship = await _friendshipRepo.FindOneAsync(fr =>
            fr.UserId1 == userId && fr.UserId2 == user.Id || fr.UserId1 == user.Id && fr.UserId2 == userId);
        if (friendship == null)
            return NotFound();
        await _friendshipRepo.RemoveAsync(friendship);
        return Ok();
    }

    [ProducesResponseType<List<FriendProfileDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet("friends")]
    public async Task<IActionResult> GetFriendList()
    {
        var userId = User.FindFirst("sub")?.Value;
        if (userId == null)
            return Unauthorized();
        var friendships = await _friendshipRepo.FindAllAsync(f => f.UserId1 == userId || f.UserId2 == userId);
        var friendProfiles = new List<FriendProfileDto>();
        foreach (var friendship in friendships)
        {
            var id = friendship.UserId1 == userId ? friendship.UserId2 : friendship.UserId1;
            var fp = await _userProfileRepo.FindOneAsync(up => up.Id == id);
            var user = await _userRepo.GetAsync(id);
            var res = new FriendProfileDto()
            {
                Username = user!.Username,
                AvatarId = fp!.ProfileSkin,
                Level = fp.Level,
                RocketShipId = fp.RocketSkin,
                CurrentStreak = await _historyUtils.GetCurrentStreak(fp)
            };
            friendProfiles.Add(res);
        }

        return Ok(friendProfiles);
    }
}