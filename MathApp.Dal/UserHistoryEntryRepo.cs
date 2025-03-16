using MathApp.Dal.Interfaces;
using Models;

namespace Dal;

public class UserHistoryEntryRepo : BaseRepo<UserHistoryEntry>, IUserHistoryEntryRepo
{
    public UserHistoryEntryRepo(MathAppDbContext db) : base(db)
    {
    }

    public override Task AddAsync(UserHistoryEntry entity)
    {
        if(entity.Id == "")
        {
            entity.Id = Guid.NewGuid().ToString();
        }
        return base.AddAsync(entity);
    }
}
