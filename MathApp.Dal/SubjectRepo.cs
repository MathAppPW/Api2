using MathApp.Dal.Interfaces;
using Models;

namespace Dal;

public class SubjectRepo : BaseRelatedRepo<Subject>, ISubjectRepo
{
    public SubjectRepo(MathAppDbContext db) : base(db)
    {
    }
}