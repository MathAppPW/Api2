using System.Linq.Expressions;

namespace MathApp.Dal.Interfaces;

public interface IRepo<T> where T : new()
{
    Task AddAsync(T entity);
    Task<bool> AllAsync(Expression<Func<T, bool>> predicate);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    Task<T?> GetAsync(object id);
    Task<List<T>> GetAllAsync();
    Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate);
    Task<List<T>> FindAllAsync(Expression<Func<T, bool>> predicate);
    Task RemoveAsync(T entity);
    Task UpdateAsync(T entity);
}