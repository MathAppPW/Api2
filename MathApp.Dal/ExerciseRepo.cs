using MathApp.Dal.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dal;

public class ExerciseRepo : BaseRelatedRepo<Models.Exercise>, IExerciseRepo
{
    public ExerciseRepo(MathAppDbContext dbContext) : base(dbContext)
    {
    }
}
