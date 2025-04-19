using System.Linq.Expressions;
using MathApp.Dal.Interfaces;
using Microsoft.EntityFrameworkCore;
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

    public override async Task<List<Friendship>> FindAllAsync(Expression<Func<Friendship, bool>> predicate)
    {
        return await Table.Where(predicate).Include(f => f.User1).Include(f => f.User2).ToListAsync();
    }

    public override async Task<Friendship?> FindOneAsync(Expression<Func<Friendship, bool>> predicate)
    {
        var friendship = await Table.FirstOrDefaultAsync(predicate);
        if (friendship is null)
            return friendship;
        await LoadMemberAsync(friendship, f => f.User1);
        await LoadMemberAsync(friendship, f => f.User2);
        return friendship;
    }

    public override async Task<Friendship?> GetAsync(object id)
    {
        return await Table.Include(f => f.User1).Include(f => f.User2).FirstOrDefaultAsync(f => f.Id.Equals(id));
    }
}