using MathApp.Dal.Interfaces;
using Models;

namespace Dal;

public class UserProfileRepo : BaseRelatedRepo<UserProfile>, IUserProfileRepo
{
    public UserProfileRepo(MathAppDbContext db) : base(db)
    {
    }

    public override Task AddAsync(UserProfile entity)
    {
        if(entity.Id == "")
        {
            entity.Id = Guid.NewGuid().ToString();
        }
        return base.AddAsync(entity);
    }
}
