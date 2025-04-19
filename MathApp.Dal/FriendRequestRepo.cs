using System.Linq.Expressions;
using MathApp.Dal.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Dal;

public class FriendRequestRepo : BaseRelatedRepo<FriendRequest>, IFriendRequestRepo
{
    public FriendRequestRepo(MathAppDbContext db) : base(db)
    {
    }

    public override Task<List<FriendRequest>> FindAllAsync(Expression<Func<FriendRequest, bool>> predicate)
    {
        return Table.Include(fr => fr.Receiver).Include(fr => fr.Sender).Where(predicate).ToListAsync();
    }

    public override Task<FriendRequest?> FindOneAsync(Expression<Func<FriendRequest, bool>> predicate)
    {
        return Table.Include(fr => fr.Receiver).Include(fr => fr.Sender).FirstOrDefaultAsync(predicate);
    }

    public override Task<FriendRequest?> GetAsync(object id)
    {
        return Table.Include(fr => fr.Receiver).Include(fr => fr.Sender).FirstOrDefaultAsync(fr => fr.Id.Equals(id));
    }
}