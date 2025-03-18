using MathApp.Dal.Interfaces;
using Models;

namespace Dal;

public class ExerciseSeriesRepo : BaseRelatedRepo<ExerciseSeries>, IExerciseSeriesRepo
{
    public ExerciseSeriesRepo(MathAppDbContext db) : base(db)
    {
    }
}