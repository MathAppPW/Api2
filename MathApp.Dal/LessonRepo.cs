using MathApp.Dal.Interfaces;
using Models;

namespace Dal;

public class LessonRepo : BaseRelatedRepo<Lesson>, ILessonRepo
{
    public LessonRepo(MathAppDbContext db) : base(db)
    {
    }
}