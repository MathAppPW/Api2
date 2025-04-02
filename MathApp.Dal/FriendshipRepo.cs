using MathApp.Dal.Interfaces;
using Models;

namespace Dal;

public class FriendshipRepo : BaseRelatedRepo<Friendship>, IFriendshipRepo
{
    public FriendshipRepo(MathAppDbContext db) : base(db)
    {
    }
}