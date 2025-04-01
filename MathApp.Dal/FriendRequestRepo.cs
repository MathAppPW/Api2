using MathApp.Dal.Interfaces;
using Models;

namespace Dal;

public class FriendRequestRepo : BaseRelatedRepo<FriendRequest>, IFriendRequestRepo
{
    public FriendRequestRepo(MathAppDbContext db) : base(db)
    {
    }
}