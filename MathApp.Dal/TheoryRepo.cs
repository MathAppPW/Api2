using MathApp.Dal.Interfaces;
using Models;

namespace Dal;

public class TheoryRepo : BaseRepo<Theory>, ITheoryRepo
{
    public TheoryRepo(MathAppDbContext db) : base(db)
    {
    }

    public override Task AddAsync(Theory entity)
    {
        if(entity.Id == "")
        {
            entity.Id = Guid.NewGuid().ToString();
        }
        return base.AddAsync(entity);
    }
}
