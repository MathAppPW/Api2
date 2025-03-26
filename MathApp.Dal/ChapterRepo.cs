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

public class ChapterRepo : BaseRelatedRepo<Models.Chapter>, IChapterRepo
{
    public ChapterRepo(MathAppDbContext dbContext) : base(dbContext)
    {
    }
}
