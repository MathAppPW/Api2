using MathApp.Dal.Interfaces;
using Models;

namespace Dal;

public class ExerciseRepo : BaseRelatedRepo<Exercise>, IExerciseRepo
{
    public ExerciseRepo(MathAppDbContext db) : base(db)
    {
    }
}