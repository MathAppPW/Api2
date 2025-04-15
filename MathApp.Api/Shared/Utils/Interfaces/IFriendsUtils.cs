using Models;

namespace MathAppApi.Shared.Utils.Interfaces;

public interface IFriendsUtils
{
    Task<List<User>> GetFriends(string userId);
}