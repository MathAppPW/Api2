using MathApp.Dal.Interfaces;
using Models;

namespace Dal;

public class SeriesRepo : BaseRelatedRepo<Series>, ISeriesRepo
{
    public SeriesRepo(MathAppDbContext db) : base(db)
    {
    }
}