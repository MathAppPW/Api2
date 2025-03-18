using MathApp.Dal.Interfaces;
using Models;

namespace Dal;

public class ChapterRepo : BaseRelatedRepo<Chapter>, IChapterRepo
{
    public ChapterRepo(MathAppDbContext db) : base(db)
    {
    }
}