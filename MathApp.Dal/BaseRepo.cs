using System.Linq.Expressions;
using MathApp.Dal.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dal;

public abstract class BaseRepo<T> : IRepo<T> where T : class, new()
{
    protected DbSet<T> Table { get; }
    protected DbContext Db { get; }

    protected BaseRepo(MathAppDbContext db)
    {
        Db = db;
        Table = Db.Set<T>();
    }
    
    public virtual async Task AddAsync(T entity)
    {
        await Table.AddAsync(entity);
        await SaveChangesAsync();
    }

    public async Task<bool> AllAsync(Expression<Func<T, bool>> predicate)
    {
        return await Table.AllAsync(predicate);
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await Table.AnyAsync(predicate);
    }

    public virtual async Task<T?> GetAsync(object id)
    {
        return await Table.FindAsync(id);
    }

    public virtual async Task<List<T>> GetAllAsync()
    {
        return await Table.ToListAsync();
    }

    public virtual async Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate)
    {
        return await Table.FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<List<T>> FindAllAsync(Expression<Func<T, bool>> predicate)
    {
        return await Table.Where(predicate).ToListAsync();
    }

    public virtual async Task RemoveAsync(T entity)
    {
        Db.Remove(entity);
        await SaveChangesAsync();
    }

    public virtual async Task UpdateAsync(T entity)
    {
        Db.Update(entity);
        await Db.SaveChangesAsync();
    }

    protected async Task SaveChangesAsync()
        => await Db.SaveChangesAsync();
}