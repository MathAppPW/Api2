using MathApp.Dal.Interfaces;
using Models;

namespace Dal;

public class UserProfileRepo : BaseRepo<UserProfile>, IUserProfileRepo
{
    public UserProfileRepo(MathAppDbContext db) : base(db)
    {
    }

    public override Task AddAsync(UserProfile entity)
    {
        entity.Id = Guid.NewGuid().ToString();
        return base.AddAsync(entity);
    }
}
