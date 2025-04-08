using MathApp.Dal.Interfaces;
using Models;

namespace Dal;

public class FriendshipRepo : BaseRelatedRepo<Friendship>, IFriendshipRepo
{
    public FriendshipRepo(MathAppDbContext db) : base(db)
    {
    }

    public override Task AddAsync(Friendship entity)
    {
        if (entity.Id == "")
        {
            entity.Id = Guid.NewGuid().ToString();
        }
        return base.AddAsync(entity);
    }
}