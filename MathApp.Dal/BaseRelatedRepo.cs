using System.Linq.Expressions;
using MathApp.Dal.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dal;

public abstract class BaseRelatedRepo<T> : BaseRepo<T>, IRelatedRepo<T> where T : class, new()
{
    protected BaseRelatedRepo(MathAppDbContext db) : base(db)
    {
    }

    public async Task LoadMemberAsync<TProperty>(T entity, Expression<Func<T, TProperty?>> selector)
        where TProperty : class
    {
        await Table.Entry(entity)
            .Reference(selector)
            .LoadAsync();
    }

    public async Task LoadCollectionAsync<TProperty>(T entity, Expression<Func<T, IEnumerable<TProperty>>> selector)
        where TProperty : class
    {
        await Table.Entry(entity)
            .Collection(selector)
            .LoadAsync();
    }

    public async Task<int> CountCollectionAsync<TProperty>(T entity,
        Expression<Func<T, IEnumerable<TProperty>>> selector)
        where TProperty : class
    {
        return await Table.Entry(entity)
            .Collection(selector)
            .Query()
            .CountAsync();
    }
}