using MathApp.Dal.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Dal;

public class UserRepo : BaseRepo<User>, IUserRepo
{
    public UserRepo(MathAppDbContext db) : base(db)
    {
    }

    public override Task AddAsync(User entity)
    {
        entity.Id = Guid.NewGuid().ToString();
        return base.AddAsync(entity);
    }
}