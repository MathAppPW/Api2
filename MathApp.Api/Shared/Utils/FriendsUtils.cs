using MathApp.Dal.Interfaces;
using MathAppApi.Shared.Utils.Interfaces;
using Models;

namespace MathAppApi.Shared.Utils;

public class FriendsUtils : IFriendsUtils
{
    private readonly IFriendshipRepo _friendshipRepo;
    private readonly IUserRepo _userRepo;

    public FriendsUtils(IFriendshipRepo friendshipRepo, IUserRepo userRepo)
    {
        _friendshipRepo = friendshipRepo;
        _userRepo = userRepo;
    }
    
    public async Task<List<User>> GetFriends(string userId)
    {
        var friendships = await _friendshipRepo.FindAllAsync(fr => fr.UserId1 == userId || fr.UserId2 == userId);
        var result = new List<User>();
        foreach (var f in friendships)
        {
            var uId = f.UserId1 != userId ? f.UserId1 : f.UserId2;
            var user = await _userRepo.GetAsync(uId);
            if (user != null)
                result.Add(user);
        }

        return result;
    }
}